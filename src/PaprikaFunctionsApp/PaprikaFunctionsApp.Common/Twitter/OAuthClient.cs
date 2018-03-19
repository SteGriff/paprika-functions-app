using PaprikaFunctionsApp.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace PaprikaFunctionsApp.Common.Twitter
{
    public class OAuthClient : IOAuthClient
    {
        private ITraceWriter log;

        public OAuthClient(ITraceWriter logger)
        {
            
            log = logger;
        }

        public Uri GetAuthorizeUri(string consumerKey, string consumerSecret, Uri callback)
        {
            log.Info("GetAuthorizeUri " + consumerKey + " :: " + consumerSecret + " :: " + callback.ToString());
            return
                new Uri("https://api.twitter.com/oauth/authorize?oauth_token=" +
                        GetOAuthToken(consumerKey, consumerSecret, callback));
        }

        public TwitterUser GetUser(string consumerKey, string consumerSecret, string oauthToken, string oauthVerifier)
        {
            log.Info("GetUser " + consumerKey + " :: " + consumerSecret + " :: " + oauthToken + " :: " + oauthVerifier);

            var oauthParameters = new OAuthParameterSet(consumerKey, consumerSecret, oauthToken)
                                      {
                                          {OAuthParameter.Verifier, oauthVerifier}
                                      };
            string response;
            using (var webClient = new WebClient())
            {
                response = webClient.DownloadString(GetAccessTokenUri(), oauthParameters);
            }
            if (response == null) throw new InvalidOperationException("Failed to get user");

            log.Info(response);

            Dictionary<string, string> values =
                    response.Split('&').Select(section => section.Split('=')).ToDictionary(
                        bits => bits[0], bits => bits[1]);

            return new TwitterUser
            {
                Token = values["oauth_token"],
                TokenSecret = values["oauth_token_secret"],
                UserId = long.Parse(values["user_id"]),
                ScreenName = values["screen_name"]
            };
        }

        private Uri GetRequestTokenUri(Uri callback)
        {
            log.Info("GetRequestTokenUri " + callback.ToString());

            return
                new Uri("https://api.twitter.com/oauth/request_token?oauth_callback=" + callback.ToString().UrlEncode());
        }

        private Uri GetAccessTokenUri()
        {
            log.Info("GetAccessTokenUri ");

            return new Uri("https://api.twitter.com/oauth/access_token");
        }

        private string GetOAuthToken(string consumerKey, string consumerSecret, Uri callback)
        {
            log.Info("GetOAuthToken " + consumerKey + " :: " + consumerSecret + " :: " + callback.ToString());

            var oauthParameters = new OAuthParameterSet(consumerKey, consumerSecret)
                                      {
                                          {OAuthParameter.Callback, callback.ToString()},
                                      };

            string response;
            using (var webClient = new WebClient())
            {
                response = webClient.UploadString(GetRequestTokenUri(callback), string.Empty, oauthParameters);
            }

            if (response == null) throw new InvalidOperationException("Failed to get OAuth token");

            Dictionary<string, string> values =
                response.Split('&').Select(section => section.Split('=')).ToDictionary(
                    bits => bits[0], bits => bits[1]);
            return values["oauth_token"];
        }
    }
}
