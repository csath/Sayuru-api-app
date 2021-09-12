using FirebaseAdmin.Messaging;
using Sayuru.Mobile.API.Data.Interfaces;
using Sayuru.Mobile.API.Data.Models;
using System;
using System.Threading.Tasks;

namespace Sayuru.Mobile.API.Helpers
{
    public class PushNotificationHelper
    {
        IApplicationSettings _settings;
        Logger _logger;

        public PushNotificationHelper(IApplicationSettings settings, Logger logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task<string> SendNotification(PushNotificationPayload msg)
        {
            try
            {
                var message = new Message()
                {
                    Data = msg.Data,
                    Notification = new Notification
                    {
                        Title = msg.Title,
                        Body = msg.Body
                    },
                    Topic = msg.Topic
                };

                var messaging = FirebaseMessaging.DefaultInstance;
                var result = await messaging.SendAsync(message);

                return result;
            }
            catch(Exception ex)
            {
                _logger.Log(exception: ex);
                return "";
            }
        }
    }
}
