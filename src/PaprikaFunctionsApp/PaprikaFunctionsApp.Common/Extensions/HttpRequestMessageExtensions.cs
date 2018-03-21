using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace PaprikaFunctionsApp.Common.Extensions
{
    public static class HttpRequestMessageExtensions
    {
        public static void AddAuthenticationHeadersFromIdentifier
            (this HttpRequestMessage req, string identifier)
        {
            string username = "";
            string password = "";
            try
            {
                string plainIdentifier = identifier.Base64Decode();
                var idParts = plainIdentifier.Split(new string[] { "----IDENTIFIER----" }, StringSplitOptions.RemoveEmptyEntries);
                username = idParts[0];
                password = idParts[1];
            }
            catch (Exception)
            {
                //Just Return; req will not have headers added and will be unauthenticated
                return;
            }
            
            req.Headers.Add("username", username);
            req.Headers.Add("password", password);
        }

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }
}
