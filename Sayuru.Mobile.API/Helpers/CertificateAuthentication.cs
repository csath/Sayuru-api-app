using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Sayuru.Mobile.API.Interfaces;
using Sayuru.Mobile.API.Models;

namespace Sayuru.Mobile.API.Helpers
{
    public class CertificateAuthentication
    {
        IApplicationSettings _settings;
        IVRApi _iVRApi;

        public CertificateAuthentication(IApplicationSettings settings)
        {
            _settings = settings;
        }

        public bool ValidateCertificate(X509Certificate2 clientCertificate)
        {
            string[] allowedThumbprints = _settings.AllowedCertificateThumbPrints.Split(",");
             
            if (allowedThumbprints.Contains(clientCertificate.Thumbprint))
            {
                return true;
            }

            return false;
        }

        public string GenerateUserToken(string userId)
        {
            return CreateMD5($"{userId}");
        }

        public bool CheckIfUserTokenIsValid(string userId, string token)
        {
            return CreateMD5($"{userId}") == token;
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
