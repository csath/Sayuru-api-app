using Newtonsoft.Json;
using Sayuru.Mobile.API.Data.Common;
using Sayuru.Mobile.API.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sayuru.Mobile.API.Helpers
{
    public class IVRApi
    {
        IApplicationSettings _settings;
        Logger _logger;
        string IVR_Username;
        string IVR_Digest;

        public IVRApi(IApplicationSettings settings, Logger logger)
        {
            _settings = settings;
            _logger = logger;
            IVR_Username = settings.IVRAPIUsername;
            IVR_Digest = CreateMD5(settings.IVRAPIPassword);
        }

        public bool RegisterForIVR(long phoneNumber)
        {
            try
            {
                string phoneNo = phoneNumber.ToString().Trim();
                string check94 = phoneNumber.ToString().Substring(0, 2);
                if (check94 != "94")
                {
                    phoneNo = "94" + phoneNumber.ToString().Trim();
                }

                string uri = "/api/app/register";
                var data = new
                {
                    number = phoneNo
                };
                var stringContent = new StringContent(data.ToString());

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_settings.IVRAPIBaseURL);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("USER", IVR_Username);
                    client.DefaultRequestHeaders.Add("DIGEST", IVR_Digest);
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var task = Task.Run(() => client.PostAsync(uri, stringContent));
                    task.Wait();
                    var response = task.Result;

                    var task1 = Task.Run(() => response.Content.ReadAsStringAsync());
                    task1.Wait();

                    var res = JsonConvert.DeserializeObject<RegisterRes>(task1.Result);

                    if (response.IsSuccessStatusCode && res.ResultCode == 0)
                    {
                        return true;
                    }
                }

                return false;
            }

            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return false;
            }
        }

        public bool GetIVRRegisterdStatus(long phoneNumber)
        {
            try
            {
                string phoneNo = phoneNumber.ToString().Trim();
                string check94 = phoneNumber.ToString().Substring(0, 2);
                if (check94 != "94")
                {
                    phoneNo = "94" + phoneNumber.ToString().Trim();
                }

                string uri = "/api/app/getstatus";
                var data = new
                {
                    number = phoneNo
                };
                var stringContent = new StringContent(data.ToString());

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_settings.IVRAPIBaseURL);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("USER", IVR_Username);
                    client.DefaultRequestHeaders.Add("DIGEST", IVR_Digest);
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var task = Task.Run(() => client.PostAsync(uri, stringContent));
                    task.Wait();
                    var response = task.Result;

                    var task1 = Task.Run(() => response.Content.ReadAsStringAsync());
                    task1.Wait();

                    var res = JsonConvert.DeserializeObject<RegisterRes>(task1.Result);

                    if (response.IsSuccessStatusCode && res.ResultCode == 0 && res.Status)
                    {
                        return true;
                    }
                }

                return false;
            }

            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return false;
            }
        }

        public List<Zone> GetZones()
        {
            try
            {
                string uri = "/api/app/getzones";

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_settings.IVRAPIBaseURL);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("USER", IVR_Username);
                    client.DefaultRequestHeaders.Add("DIGEST", IVR_Digest);
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var task = Task.Run(() => client.GetAsync(uri));
                    task.Wait();
                    var response = task.Result;

                    var task1 = Task.Run(() => response.Content.ReadAsStringAsync());
                    task1.Wait();

                    var res = JsonConvert.DeserializeObject<ZoneRes>(task1.Result);

                    if (response.IsSuccessStatusCode && res.ResultCode == 0)
                    {
                        return res.Zones;
                    }
                }

                return new List<Zone> { };
            }

            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return new List<Zone> { };
            }
        }

        public PredictionRes GetPredictionsForLocation(Location location)
        {
            try
            {
                string uri = "/api/app/getpredictions";
                var data = new
                {
                    lat = location.Lat,
                    lon = location.Lon
                };
                var stringContent = new StringContent(data.ToString());

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_settings.IVRAPIBaseURL);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("USER", IVR_Username);
                    client.DefaultRequestHeaders.Add("DIGEST", IVR_Digest);
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var task = Task.Run(() => client.GetAsync(uri));
                    task.Wait();
                    var response = task.Result;

                    var task1 = Task.Run(() => response.Content.ReadAsStringAsync());
                    task1.Wait();

                    var res = JsonConvert.DeserializeObject<PredictionRes>(task1.Result);

                    if (response.IsSuccessStatusCode && res.ResultCode == 0)
                    {
                        return res;
                    }
                }

                return null;
            }

            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return null;
            }
        }

        public int SendOtp(long phoneNumber)
        {
            try
            {
                var phoneNo = phoneNumber.ToString();

                string check94 = phoneNumber.ToString().Substring(0, 2);
                if (check94 != "94")
                {
                    phoneNo = "94" + phoneNumber.ToString().Trim();
                }

                string uri = "/api/app/sendotp";
                var data = new
                {
                    number = phoneNo,
                    otp = GenerateOTP()
                };
                var stringContent = new StringContent(data.ToString());

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_settings.IVRAPIBaseURL);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("USER", IVR_Username);
                    client.DefaultRequestHeaders.Add("DIGEST", IVR_Digest);
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var task = Task.Run(() => client.PostAsync(uri, stringContent));
                    task.Wait();
                    var response = task.Result;

                    var task1 = Task.Run(() => response.Content.ReadAsStringAsync());
                    task1.Wait();

                    var res = JsonConvert.DeserializeObject<RegisterRes>(task1.Result);

                    if (response.IsSuccessStatusCode && res.ResultCode == 0)
                    {
                        return data.otp;
                    }
                }

                return 0;
            }

            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return 0;
            }
        }

        public Fisherman GetFishermanData(string nic)
        {
            try
            {
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["username"] = _settings.FishermanAPIUsername;
                query["password"] = _settings.FishermanAPIPassword;
                query["nic"] = nic;

                var stringContent = new StringContent(string.Empty);

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_settings.FishermanAPIBaseURL);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var task = Task.Run(() => client.PostAsync($"?{query.ToString()}", stringContent));
                    task.Wait();
                    var response = task.Result;

                    var task1 = Task.Run(() => response.Content.ReadAsStringAsync());
                    task1.Wait();

                    var res = JsonConvert.DeserializeObject<FishermanRes>(task1.Result);

                    if (response.IsSuccessStatusCode && res.Status == "success")
                    {
                        return res.Fishermans_data;
                    }
                }

                return null;
            }

            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                return null;
            }
        }

        public int GenerateOTP()
        {
            return new Random().Next(9000) + 1000;
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }
    }
}
