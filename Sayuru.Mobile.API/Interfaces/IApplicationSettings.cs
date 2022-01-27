using System;
using System.Collections.Generic;
using System.Text;

namespace Sayuru.Mobile.API.Interfaces
{
    public interface IApplicationSettings
    {
        string LoggerPath { get; set; }
        int OTPExpiration { get; set; }
        string IVRAPIBaseURL { get; set; }
        string IVRAPIUsername { get; set; }
        string IVRAPIPassword { get; set; }
        string FishermanAPIBaseURL { get; set; }
        string FishermanAPIUsername { get; set; }
        string FishermanAPIPassword { get; set; }
        string SMSBaseURL { get; set; }
        string SMSAPIKey { get; set; }
        string AllowedCertificateThumbPrints { get; set; }
    }
}
