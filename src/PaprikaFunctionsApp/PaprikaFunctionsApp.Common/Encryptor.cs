using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PaprikaFunctionsApp.Common
{
    public static class Encryptor
    {
        public static string Encrypt(string plainPasswordString, string salt)
        {
            var saltBytes = Encoding.UTF8.GetBytes(salt);

            var crypt = new Rfc2898DeriveBytes(plainPasswordString, saltBytes, 1000);
            var key = crypt.GetBytes(256);

            return Convert.ToBase64String(key);
        }
    }
}
