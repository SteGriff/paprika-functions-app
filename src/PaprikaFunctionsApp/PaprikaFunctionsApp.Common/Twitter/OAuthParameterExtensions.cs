using System;
using System.Net;

namespace PaprikaFunctionsApp.Common.Twitter
{
    public static class OAuthRequestExtensions
    {
        public static string UploadString(this WebClient webClient, Uri uri, string body, OAuthParameterSet parameterSet)
        {
            webClient.Headers.Set("Authorization", parameterSet.GetOAuthHeaderString(uri, "POST"));
            return webClient.UploadString(uri, body);
        }

        public static string DownloadString(this WebClient webClient, Uri uri, OAuthParameterSet parameterSet)
        {
            webClient.Headers.Set("Authorization", parameterSet.GetOAuthHeaderString(uri, "GET"));
            return webClient.DownloadString(uri);
        }

        public static void DownloadStringAsync(this WebClient webClient, Uri uri, OAuthParameterSet parameterSet)
        {
            webClient.Headers.Set("Authorization", parameterSet.GetOAuthHeaderString(uri, "GET"));
            webClient.DownloadStringAsync(uri);
        }

        public static void UploadStringAsync(this WebClient webClient, Uri uri, string body, OAuthParameterSet parameterSet)
        {
            webClient.Headers.Set("Authorization", parameterSet.GetOAuthHeaderString(uri, "POST"));
            webClient.UploadStringAsync(uri, body);
        }

        public static string UrlEncode(this string source)
        {
            return Uri.EscapeDataString(source);
        }

        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTime(this DateTime target)
        {
            return (long)(target - UnixEpoch).TotalSeconds;
        }
    }
}
