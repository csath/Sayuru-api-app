using Sayuru.Mobile.API.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sayuru.Mobile.API.Data.Models
{
    public class UserAlerts : MongoEntity
    {
        public string UserId { get; set; }
        public string TimeStamp { get; set; }
        public NotificationPayload Payload { get; set; }
    }

    public class NotificationPayload
    {
        public AlertType AlertType { get; set; }
        public string En_message { get; set; }
        public string Si_message { get; set; }
        public string Ta_message { get; set; }
    }

    public class PushNotificationPayload
    {
        public Dictionary<string, string> Data { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Topic { get; set; }
    }

    public class ExternalAlert
    {
        public List<Zone> Zones { get; set; }
        public AlertType AlertType { get; set; }
        public string En_message { get; set; }
        public string Si_message { get; set; }
        public string Ta_message { get; set; }
    }
}
