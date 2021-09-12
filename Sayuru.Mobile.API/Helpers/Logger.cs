using Sayuru.Mobile.API.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sayuru.Mobile.API.Helpers
{
    public class Logger
    {
        IApplicationSettings _settings;

        public Logger(IApplicationSettings settings)
        {
            _settings = settings;
        }

        public void Log(string message = null, Exception exception = null)
        {
            string executableLocation = Path.GetDirectoryName(_settings.LoggerPath);
            string filePath = @"Logs";
            filePath = Path.Combine(executableLocation, filePath);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            filePath = Path.Combine(filePath, $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}");

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.UtcNow.ToString());
                writer.WriteLine();

                while (exception != null)
                {
                    writer.WriteLine(exception.GetType().FullName);
                    writer.WriteLine("Message : " + exception.Message);
                    writer.WriteLine("StackTrace : " + exception.StackTrace);
                    exception = exception.InnerException;
                }

                if (!string.IsNullOrEmpty(message))
                {
                    writer.WriteLine(message);
                }
            }
        }
    }
}
