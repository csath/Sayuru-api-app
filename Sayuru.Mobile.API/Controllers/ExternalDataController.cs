using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sayuru.Mobile.API.Common;
using Sayuru.Mobile.API.Interfaces;
using Sayuru.Mobile.API.Models;
using Sayuru.Mobile.API.Helpers;

namespace Sayuru.Mobile.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalDataController : ControllerBase
    {
        IDBContext _dbContext;
        Logger _logger;
        IVRApi _ivrAPI;

        public ExternalDataController(IDBContext dbContext, Logger logger, IVRApi ivrAPI)
        {
            _dbContext = dbContext;
            _logger = logger;
            _ivrAPI = ivrAPI;
        }

        #region Zones

        [HttpGet("zones")]
        public Response<List<Zone>> GetZones()
        {
            return new Response<List<Zone>> { Data = _ivrAPI.GetZones() };
        }

        #endregion

        #region DOF

        [HttpGet("dof")]
        public Response<DOF> GetDOFInfo()
        {
            try
            {
                var theFilter = Builders<DOF>.Filter.Eq(dof => dof.Id, "5e0e6a804888946fa61a1976");
                var data = _dbContext.DOF.FindSync<DOF>(theFilter).FirstOrDefault();

                return new Response<DOF> { Data = data };
            }
            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return new Response<DOF>
                {
                    IsSuccess = false,
                    Code = 400,
                    Error = "Unable to fetch dof data"
                };
            }
        }

        [HttpPost("dof")]
        public Response<DOF> UpdateDOF([FromBody] DOF dof)
        {
            try
            {
                dof.Id = "5e0e6a804888946fa61a1976";
                var filter = Builders<DOF>.Filter.Eq(user => user.Id, dof.Id);
                var update = Builders<DOF>.Update
                    .Set(user => user.Images, dof.Images)
                    .Set(user => user.ButtonLinks, dof.ButtonLinks)
                    .Set(user => user.WebViewURL, dof.WebViewURL);
                var options = new UpdateOptions { IsUpsert = true };
                _dbContext.DOF.UpdateOne(filter, update, options);

                return new Response<DOF> { Data = dof };
            }
            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return new Response<DOF>
                {
                    IsSuccess = false,
                    Code = 400,
                    Error = "Unable to update dof data"
                };
            }
        }

        #endregion

        #region WeatherData

        [HttpPost("weathercurrent")]
        public Response<PredictionRes> GetWeatherDataForCurrentLocation([FromBody] Location location)
        {
            try
            {
                var res = _ivrAPI.GetPredictionsForLocation(location);

                if (res == null)
                {
                    return new Response<PredictionRes>
                    {
                        IsSuccess = false,
                        Code = 400,
                        Error = "Unable to fetch data"
                    };
                }

                return new Response<PredictionRes>
                {
                    Data = res
                };
            }
            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return new Response<PredictionRes>
                {
                    IsSuccess = false,
                    Code = 400,
                    Error = "Unable to fetch data"
                };
            }
        }

        #endregion

        [HttpPost("account/status")]
        public Response<AccountStatus> GetAccountFreeTrialStatus([FromBody] UserData userData)
        {
            try
            {
                var res = _ivrAPI.GetAccountStatus(userData.MobileNumber);

                if (res == null)
                {
                    return new Response<AccountStatus>
                    {
                        IsSuccess = false,
                        Code = 400,
                        Error = "Unable to fetch data"
                    };
                }

                return new Response<AccountStatus>
                {
                    Data = res
                };
            }
            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return new Response<AccountStatus>
                {
                    IsSuccess = false,
                    Code = 400,
                    Error = "Unable to fetch data"
                };
            }
        }

    }
}
