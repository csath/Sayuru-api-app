using Sayuru.Mobile.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Sayuru.Mobile.API.Helpers
{
    public class SMSHelper
    {
        IApplicationSettings _settings;
        Logger _logger;

        public SMSHelper(IApplicationSettings settings, Logger logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public bool SendSMS(string message, long phoneNumber)
        {
            try
            {
                string check94 = phoneNumber.ToString().Substring(0, 2);

                if (check94 != "94")
                {
                    string phoneNo = "94" + phoneNumber.ToString().Trim();
                    phoneNumber = Convert.ToInt64(phoneNo);
                }

                string uri = GetURI(message, phoneNumber);

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_settings.SMSBaseURL);
                    client.DefaultRequestHeaders.Clear();
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var task = Task.Run(() => client.GetAsync($"?{uri}"));
                    task.Wait();
                    var response = task.Result;
                    if (response.IsSuccessStatusCode)
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

        public int SendOTP(long phoneNumber)
        {
            int otp;
            try
            {
                otp = GenerateOTP();
                if (!SendSMS($"Use this OTP to login to the Sayuru App: {otp}", phoneNumber))
                {
                    otp = 0;
                }
            }

            catch (Exception ex)
            {
                _logger.Log(exception: ex);
                otp = 0;
            }

            return otp;
        }

        public int GenerateOTP()
        {
            return new Random().Next(9000) + 1000;
        }

        private string GetURI(string message, long phoneNumber)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["destination"] = phoneNumber.ToString();
            query["q"] = _settings.SMSAPIKey;
            query["message"] = message;
            return query.ToString();
        }
    }
}
