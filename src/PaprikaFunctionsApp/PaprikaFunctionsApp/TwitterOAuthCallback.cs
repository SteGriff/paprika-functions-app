using System.Linq;
using System;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Configuration;
using PaprikaFunctionsApp.Common.Twitter;
using PaprikaFunctionsApp.Adaptors;
using PaprikaFunctionsApp.Common;

namespace PaprikaFunctionsApp
{
    public static class TwitterOAuthCallback
    {
        private static AzureStorageProvider _storageProvider;

        [FunctionName("TwitterOAuthCallback")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Twitter/OAuth")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("OAuth Response");

            try
            {
                _storageProvider = StorageProvider.GetStorageProvider();
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Storage Connection Error");
            }

            var requestValues = req.GetQueryNameValuePairs();

            string oauthToken = requestValues
                .FirstOrDefault(q => string.Compare(q.Key, "oauth_token", true) == 0)
                .Value;

            string oauthVerifier = requestValues
                .FirstOrDefault(q => string.Compare(q.Key, "oauth_verifier", true) == 0)
                .Value;

            var consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            var consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
            var twitterCallback = ConfigurationManager.AppSettings["TwitterCallback"];

            var myLogger = new TraceAdaptor(log);
            var twitter = new OAuthClient(myLogger);
            var user = twitter.GetUser(consumerKey, consumerSecret, oauthToken, oauthVerifier);

            var usersData = new Users(ConfigurationManager.ConnectionStrings["PrimaryStorage"].ConnectionString);
            var userEntity = new UserEntity(user.UserName, user.UserName)
            {
                OAuthToken = user.Token,
                OAuthVerifier = oauthVerifier,
                OAuthTokenSecret = user.TokenSecret,
                UpdatedDate = DateTime.Now
            };

            var result = await usersData.InsertUserAsync(userEntity);

            if (string.IsNullOrEmpty(result))
            {
                return req.CreateResponse(HttpStatusCode.OK, "All good, go to /api/Tweet/{username}");
            }
            else
            {
                return req.CreateResponse<string>(HttpStatusCode.InternalServerError, result);
            }
        }
    }
}
