using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sayuru.Mobile.API.Data.Common;
using Sayuru.Mobile.API.Data.Interfaces;
using Sayuru.Mobile.API.Data.Models;
using Sayuru.Mobile.API.Helpers;

namespace Sayuru.Mobile.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDataController : ControllerBase
    {
        IApplicationSettings _settings;
        IDBContext _dbContext;
        Logger _logger;
        IVRApi _ivrAPI;
        SMSHelper _sMSHelper;
        JsonSerializerSettings _serializerSettings;
        PushNotificationHelper _pushNotificationHelper;

        public UserDataController(IApplicationSettings settings, IDBContext dbContext, Logger logger, IVRApi ivrAPI, SMSHelper sMSHelper, PushNotificationHelper pushNotificationHelper)
        {
            _settings = settings;
            _dbContext = dbContext;
            _logger = logger;
            _ivrAPI = ivrAPI;
            _sMSHelper = sMSHelper;
            _pushNotificationHelper = pushNotificationHelper;

            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        [HttpPost("login")]
        public Response<UserData> SendOtp([FromBody] UserData userData)
        {
            //int otp = _ivrAPI.SendOtp(userData.MobileNumber);
            int otp = _sMSHelper.SendOTP(userData.MobileNumber);

            if (otp == 0)
            {
                return new Response<UserData>
                {
                    IsSuccess = false,
                    Code = 400,
                    Error = "Cannot send otp to given number"
                };
            }

            var filter = Builders<UserSecurity>.Filter.Eq(user => user.MobileNumber, userData.MobileNumber);
            var update = Builders<UserSecurity>.Update.Set(user => user.OTP, otp)
                        .Set(user => user.OTPExpiration, DateTime.UtcNow.AddMinutes(_settings.OTPExpiration));
            var options = new UpdateOptions { IsUpsert = true };
            _dbContext.UserSecurity.UpdateOne(filter, update, options);

            var theFilter1 = Builders<UserData>.Filter.Eq(user => user.MobileNumber, userData.MobileNumber);
            var dataUser = _dbContext.UserData.FindSync<UserData>(theFilter1).FirstOrDefault();

            return new Response<UserData>
            {
                Data = new UserData
                {
                    MobileNumber = userData.MobileNumber,
                    HasRegisteredIVR = dataUser != null ? dataUser.HasRegisteredIVR : false,
                }
            };
        }

        [HttpPost("login/{otp}")]
        public Response<UserData> ConfirmOTP([FromBody] UserData userData, int otp)
        {
            try
            {
                var theFilter = Builders<UserSecurity>.Filter.Eq(user => user.MobileNumber, userData.MobileNumber);
                var data = _dbContext.UserSecurity.FindSync<UserSecurity>(theFilter).FirstOrDefault();

                if (data == null)
                {
                    return new Response<UserData>
                    {
                        IsSuccess = false,
                        Code = 404,
                        Error = "No User found for the given mobile number"
                    };
                }

                if (data.OTP != otp || DateTime.UtcNow > data.OTPExpiration)
                {
                    return new Response<UserData>
                    {
                        IsSuccess = false,
                        Code = 401,
                        Error = "OTP wrong or expired"
                    };
                }

                var theFilter1 = Builders<UserData>.Filter.Eq(user => user.MobileNumber, userData.MobileNumber);
                var dataUser = _dbContext.UserData.FindSync<UserData>(theFilter1).FirstOrDefault();

                if (dataUser == null)
                {
                    try
                    {
                        userData.Id = ObjectId.GenerateNewId().ToString();
                        userData.IsActive = false;

                        if (userData.HasRegisteredIVR)
                        {
                            userData.HasRegisteredIVR= _ivrAPI.RegisterForIVR(userData.MobileNumber);
                        }

                        _dbContext.UserData.InsertOne(userData);

                        return new Response<UserData> { Data = userData };
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(exception: ex);
                        return new Response<UserData>
                        {
                            IsSuccess = false,
                            Code = 400,
                            Error = "Unable to Register User. Please Contact Admin"
                        };
                    }
                }
                else
                {
                    if (!dataUser.HasRegisteredIVR && userData.HasRegisteredIVR)
                    {
                        dataUser.HasRegisteredIVR = _ivrAPI.RegisterForIVR(userData.MobileNumber);

                        var userUpdate = Builders<UserData>.Update
                            .Set(u => u.HasRegisteredIVR, userData.HasRegisteredIVR);

                        _dbContext.UserData.UpdateOne(theFilter1, userUpdate);
                    }

                    return new Response<UserData>
                    {
                        Data = dataUser
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return new Response<UserData>
                {
                    IsSuccess = false,
                    Code = 400,
                    Error = "Unable to confirm otp. Please Contact Admin"
                };
            }
        }

        [HttpPost("user/registerForIVR")]
        public Response<UserData> ActivateIVR([FromBody] UserData userData)
        {
            try
            {
                var theFilter = Builders<UserData>.Filter.Where(user => user.Id == userData.Id);
                var data = _dbContext.UserData.FindSync<UserData>(theFilter).FirstOrDefault();

                if (data == null)
                {
                    return new Response<UserData>
                    {
                        IsSuccess = false,
                        Code = 404,
                        Error = "No User found for the given Id"
                    };
                }

                if (data.MobileNumber != userData.MobileNumber)
                {
                    return new Response<UserData>
                    {
                        IsSuccess = false,
                        Code = 401,
                        Error = "No access to edit user record"
                    };
                }

                data.HasRegisteredIVR = _ivrAPI.RegisterForIVR(userData.MobileNumber);

                var userUpdate = Builders<UserData>.Update
                    .Set(u => u.HasRegisteredIVR, data.HasRegisteredIVR);

                _dbContext.UserData.UpdateOne(theFilter, userUpdate);

                return new Response<UserData> { Data = GetUserByMobileNumber(data.MobileNumber).Data };
            }

            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return new Response<UserData>
                {
                    IsSuccess = false,
                    Code = 400,
                    Error = "Unable to register for IVR. Please Contact Admin"
                };
            }
        }

        [HttpPost("user/update")]
        public Response<UserData> UpdateUser([FromBody] UserData userData)
        {
            try
            {
                var theFilter = Builders<UserData>.Filter.Where(user => user.Id == userData.Id);
                var data = _dbContext.UserData.FindSync<UserData>(theFilter).FirstOrDefault();

                if (data == null)
                {
                    return new Response<UserData>
                    {
                        IsSuccess = false,
                        Code = 404,
                        Error = "No User found for the given Id"
                    };
                }

                if (data.MobileNumber != userData.MobileNumber)
                {
                    return new Response<UserData>
                    {
                        IsSuccess = false,
                        Code = 401,
                        Error = "No access to edit user record"
                    };
                }

                var userUpdate = Builders<UserData>.Update
                    .Set(u => u.FirstName, userData.FirstName)
                    .Set(u => u.LastName, userData.LastName)
                    .Set(u => u.PreferedLanguage, userData.PreferedLanguage)
                    .Set(u => u.EmergencyContact, userData.EmergencyContact)
                    .Set(u => u.Zones, userData.Zones)
                    .Set(u => u.BirthDate, userData.BirthDate)
                    .Set(u => u.ProfilePicture, userData.ProfilePicture)
                    .Set(u => u.District, userData.District);

                if (!data.IsActive)
                {
                    var fisherman = _ivrAPI.GetFishermanData(userData.Nic);

                    if (fisherman != null)
                    {
                        data.FishermanID = fisherman.FishermanID;
                        data.Address = fisherman.Address;
                        data.FishermanLicenseStatus = fisherman.Active_status;
                        userUpdate.Set(u => u.FishermanID, data.FishermanID);
                        userUpdate.Set(u => u.Address, data.Address);
                        userUpdate.Set(u => u.FishermanLicenseStatus, data.FishermanLicenseStatus);
                        
                    }
                    //else // TODO: Add this if user can use app without fisherman ID
                    //{
                    //    return new Response<UserData>
                    //    {
                    //        IsSuccess = false,
                    //        Code = 400,
                    //        Error = "You are not a registered user in fisherman api"
                    //    };
                    //}
                }
                else if (string.IsNullOrEmpty(data.FishermanID)) // silently update fisherman ID if firstime failed
                {
                    var fisherman = _ivrAPI.GetFishermanData(data.Nic);

                    if (fisherman != null)
                    {
                        data.FishermanID = fisherman.FishermanID;
                        userUpdate.Set(u => u.FishermanID, data.FishermanID);
                        userUpdate.Set(u => u.Address, data.Address);
                        userUpdate.Set(u => u.FishermanLicenseStatus, data.FishermanLicenseStatus);
                    }
                }

                userUpdate.Set(u => u.IsActive, true);
                _dbContext.UserData.UpdateOne(theFilter, userUpdate);
                
                return new Response<UserData> { Data = GetUserByMobileNumber(data.MobileNumber).Data };
            }

            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return new Response<UserData>
                {
                    IsSuccess = false,
                    Code = 400,
                    Error = "Unable to update mobile. Please Contact Admin"
                };
            }
        }

        [HttpGet("user/{mobileNumber}")]
        public Response<UserData> GetUserByMobileNumber(long mobileNumber)
        {
            try
            {
                var theFilter = Builders<UserData>.Filter.Eq(user => user.MobileNumber, mobileNumber);
                var data = _dbContext.UserData.FindSync<UserData>(theFilter).FirstOrDefault();

                if (data == null)
                {
                    return new Response<UserData>
                    {
                        IsSuccess = false,
                        Code = 404,
                        Error = "No User found for the given Id"
                    };
                }

                return new Response<UserData> { Data = data };
            }
            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return new Response<UserData>
                {
                    IsSuccess = false,
                    Code = 400,
                    Error = "Unable to Get User. Please Contact Admin"
                };
            }
        }

        [HttpGet("points")]
        public Response<List<UserLocations>> GetPoints([FromQuery]string userid)
        {
            try
            {
                var theFilter = Builders<UserLocations>.Filter.Eq(user => user.UserId, userid);
                var data = _dbContext.UserLocations.FindSync<UserLocations>(theFilter).ToList().OrderByDescending(c => c.CreatedTimeStamp).ToList();

                if (data == null)
                {
                    return new Response<List<UserLocations>>
                    {
                        IsSuccess = false,
                        Code = 404,
                        Error = "No User locations found for the given Id"
                    };
                }

                return new Response<List<UserLocations>> { Data = data };
            }
            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return new Response<List<UserLocations>>
                {
                    IsSuccess = false,
                    Code = 400,
                    Error = "Unable to fetch data. Please Contact Admin"
                };
            }
        }

        [HttpPost("points/{userid}")]
        public Response<long> UpdatePoints([FromBody] List<UserLocations> points, [FromRoute] string userid)
        {
            try
            {
                points.ForEach(e =>
                {
                    var filter = Builders<UserLocations>.Filter.Eq(point => point.CreatedTimeStamp, e.CreatedTimeStamp);
                    var update = Builders<UserLocations>.Update
                                .Set(point => point.UserId, userid)
                                .Set(point => point.CreatedTimeStamp, e.CreatedTimeStamp)
                                .Set(point => point.Name, e.Name)
                                .Set(point => point.Timestamp, e.Timestamp)
                                .Set(point => point.Point, e.Point);
                    var options = new UpdateOptions { IsUpsert = true };
                    _dbContext.UserLocations.UpdateOne(filter, update, options);
                });

                var res = GetPoints(userid);
                return new Response<long>
                {
                    Data = (long)(res?.Data?.FirstOrDefault()?.CreatedTimeStamp)
                };
            }
            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return new Response<long>
                {
                    IsSuccess = false,
                    Code = 400,
                    Error = "Sync failed"
                };
            }
        }

        [HttpGet("routes")]
        public Response<List<UserRoutes>> GetRoutes([FromQuery] string userid)
        {
            try
            {
                var theFilter = Builders<UserRoutes>.Filter.Eq(user => user.UserId, userid);
                var data = _dbContext.UserRoutes.FindSync<UserRoutes>(theFilter).ToList().OrderByDescending(c => c.CreatedTimeStamp).ToList();

                if (data == null)
                {
                    return new Response<List<UserRoutes>>
                    {
                        IsSuccess = false,
                        Code = 404,
                        Error = "No User locations found for the given Id"
                    };
                }

                return new Response<List<UserRoutes>> { Data = data };
            }
            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return new Response<List<UserRoutes>>
                {
                    IsSuccess = false,
                    Code = 400,
                    Error = "Unable to fetch data. Please Contact Admin"
                };
            }
        }

        [HttpPost("routes/{userid}")]
        public Response<long> UpdateRoutes([FromBody] List<UserRoutes> points, [FromRoute] string userid)
        {
            try
            {
                points.ForEach(e =>
                {
                    var filter = Builders<UserRoutes>.Filter.Eq(point => point.CreatedTimeStamp, e.CreatedTimeStamp);
                    var update = Builders<UserRoutes>.Update
                                .Set(point => point.UserId, userid)
                                .Set(point => point.CreatedTimeStamp, e.CreatedTimeStamp)
                                .Set(point => point.Name, e.Name)
                                .Set(point => point.StartTimestamp, e.StartTimestamp)
                                .Set(point => point.EndTimestamp, e.EndTimestamp)
                                .Set(point => point.DistanceInKM, e.DistanceInKM)
                                .Set(point => point.Points, e.Points);
                    var options = new UpdateOptions { IsUpsert = true };
                    _dbContext.UserRoutes.UpdateOne(filter, update, options);
                });

                var res = GetRoutes(userid);
                return new Response<long>
                {
                    Data = (long)(res?.Data?.FirstOrDefault()?.CreatedTimeStamp)
                };
            }
            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return new Response<long>
                {
                    IsSuccess = false,
                    Code = 400,
                    Error = "Sync failed"
                };
            }
        }

        [HttpGet("activitylog")]
        public Response<List<UserActivityLog>> GetActivityLog([FromQuery] string userid)
        {
            try
            {
                var theFilter = Builders<UserActivityLog>.Filter.Eq(user => user.UserId, userid);
                var data = _dbContext.UserActivityLog.FindSync<UserActivityLog>(theFilter).ToList().OrderByDescending(c => c.CreatedTimeStamp).ToList();

                if (data == null)
                {
                    return new Response<List<UserActivityLog>>
                    {
                        IsSuccess = false,
                        Code = 404,
                        Error = "No User logs found for the given Id"
                    };
                }

                return new Response<List<UserActivityLog>> { Data = data };
            }
            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return new Response<List<UserActivityLog>>
                {
                    IsSuccess = false,
                    Code = 400,
                    Error = "Unable to fetch data. Please Contact Admin"
                };
            }
        }

        [HttpPost("activitylog/{userid}")]
        public Response<long> UpdateActivityLog([FromBody] List<UserActivityLog> points, [FromRoute] string userid)
        {
            try
            {
                points.ForEach(e =>
                {
                    var filter = Builders<UserActivityLog>.Filter.Eq(point => point.CreatedTimeStamp, e.CreatedTimeStamp);
                    var update = Builders<UserActivityLog>.Update
                                .Set(point => point.UserId, userid)
                                .Set(point => point.CreatedTimeStamp, e.CreatedTimeStamp)
                                .Set(point => point.LogType, e.LogType)
                                .Set(point => point.AdditionalInfo, e.AdditionalInfo);
                    var options = new UpdateOptions { IsUpsert = true };
                    _dbContext.UserActivityLog.UpdateOne(filter, update, options);
                });

                var res = GetActivityLog(userid);
                return new Response<long>
                {
                    Data = (long)(res?.Data?.FirstOrDefault()?.CreatedTimeStamp)
                };
            }
            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return new Response<long>
                {
                    IsSuccess = false,
                    Code = 400,
                    Error = "Sync failed"
                };
            }
        }

        [HttpGet("alerts")]
        public Response<List<UserAlerts>> GetAlerts([FromQuery] string userid)
        {
            try
            {
                var theFilter = Builders<UserAlerts>.Filter.Or(Builders<UserAlerts>.Filter.Eq(user => user.UserId, userid), Builders<UserAlerts>.Filter.Eq(user => user.UserId, "GENERAL_MSG"));
                var data = _dbContext.UserAlerts.FindSync<UserAlerts>(theFilter).ToList().OrderByDescending(c => c.TimeStamp).ToList();

                if (data == null)
                {
                    return new Response<List<UserAlerts>>
                    {
                        IsSuccess = false,
                        Code = 404,
                        Error = "No User alerts found for the given Id"
                    };
                }

                return new Response<List<UserAlerts>> { Data = data };
            }
            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return new Response<List<UserAlerts>>
                {
                    IsSuccess = false,
                    Code = 400,
                    Error = "Unable to fetch data. Please Contact Admin"
                };
            }
        }

        [HttpPost("alerts")]
        public async Task<Response<ExternalAlert>> CreateAlertAsync([FromBody] ExternalAlert alert)
        {
            try
            {
                // send notification to zones
                foreach (Zone z in alert.Zones)
                {
                   await CreatePushNotification(z.Id, alert);
                }

                // create db records for all users
                var theFilter = Builders<UserData>.Filter.ElemMatch("Zones", Builders<Zone>.Filter.In(z => z.Id, alert.Zones.Select(z => z.Id)));
                var userList = _dbContext.UserData.FindSync<UserData>(theFilter).ToList().Select(user => user.Id).ToList();

                foreach(string uid in userList)
                {
                    var userAlert = new UserAlerts
                    {
                        UserId = uid,
                        Payload = new NotificationPayload
                        {
                            AlertType = alert.AlertType,
                            En_message = alert.En_message,
                            Si_message = alert.Si_message,
                            Ta_message = alert.Ta_message,
                        }
                    };

                    CreateAlert(userAlert);
                }
                
                return new Response<ExternalAlert> { Data = alert };
            }
            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return new Response<ExternalAlert>
                {
                    IsSuccess = false,
                    Code = 400,
                    Error = "Unable to create alert. Please Contact Admin"
                };
            }
        }

        private UserAlerts CreateAlert(UserAlerts alert)
        {
            try
            {
                alert.Id = ObjectId.GenerateNewId().ToString();
                alert.TimeStamp = DateTime.UtcNow.ToString("o");
                _dbContext.UserAlerts.InsertOne(alert);

                return alert;
            }
            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return null;
            }
        }

        private async Task<string> CreatePushNotification(string zoneId, ExternalAlert externalAlert)
        {
            try
            {
                var userAlert = new UserAlerts
                {
                    UserId = "GENERAL_MSG",
                    TimeStamp = DateTime.UtcNow.ToString("o"),
                    Payload = new NotificationPayload
                    {
                        AlertType = externalAlert.AlertType,
                        En_message = externalAlert.En_message,
                        Si_message = externalAlert.Si_message,
                        Ta_message = externalAlert.Ta_message,
                    }
                };

                var msg = new PushNotificationPayload
                {
                    Title = "Sayuru alert",
                    Body = "You got a new alert. Tap to read more",
                    Topic = zoneId,
                    Data = new Dictionary<string, string>
                    {
                        ["alert"] = JsonConvert.SerializeObject(userAlert, _serializerSettings)
                    }
                };

                return await _pushNotificationHelper.SendNotification(msg);
            }
            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return null;
            }

        }
    }
}
