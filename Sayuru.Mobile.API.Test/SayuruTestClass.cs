using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sayuru.Mobile.API.Data.Settings;
using Sayuru.Mobile.API.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sayuru.Mobile.API.Test
{
    [TestClass]
    public class SayuruTestsClass
    {
        ApplicationSettings settings = new ApplicationSettings()
        {
            //SMSBaseURL = "https://richcommunication.dialog.lk/api/sms/inline/send",
            //SMSAPIKey = "75b0d5013208a40",
            OTPExpiration = 5
        };

        [TestMethod, TestCategory("Utility")]
        public void SendSMSOkay()
        {
            var smsHelper = new SMSHelper(settings, new Logger(settings));
            Assert.IsTrue(smsHelper.SendOTP(94717797609) > 0);
        }
    }
}
