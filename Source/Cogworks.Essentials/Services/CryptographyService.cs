using System;
using System.Security.Cryptography;
using Cogworks.Essentials.Services.Interfaces;

namespace Cogworks.Essentials.Services
{
    public class CryptographyService : ICryptographyService
    {
        public string GenerateRandomToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[64];
                rng.GetBytes(bytes);

                var randomToken = Convert.ToBase64String(bytes);

                return randomToken;
            }
        }
    }
}