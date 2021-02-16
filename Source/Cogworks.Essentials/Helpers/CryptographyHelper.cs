using System;
using System.Security.Cryptography;
using Cogworks.Essentials.Helpers.Interfaces;

namespace Cogworks.Essentials.Helpers
{
    public class CryptographyHelper : ICryptographyHelper
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