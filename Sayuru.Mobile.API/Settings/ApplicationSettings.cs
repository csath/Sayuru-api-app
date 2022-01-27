using Sayuru.Mobile.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sayuru.Mobile.API.Settings
{
    public class ApplicationSettings : IApplicationSettings
    {
        public string LoggerPath { get; set; }
        public int OTPExpiration { get; set; }
        public string IVRAPIBaseURL { get; set; }
        public string IVRAPIUsername { get; set; }
        public string IVRAPIPassword { get; set; }
        public string FishermanAPIBaseURL { get; set; }
        public string FishermanAPIUsername { get; set; }
        public string FishermanAPIPassword { get; set; }
        public string SMSBaseURL { get; set; }
        public string SMSAPIKey { get; set; }
        public string AllowedCertificateThumbPrints { get; set; }
    }
}
