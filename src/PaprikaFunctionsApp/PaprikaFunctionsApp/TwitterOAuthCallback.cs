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
        public static async System.Threading.Tasks.Task<HttpResponseMessage> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Twitter/OAuth/{username}")]HttpRequestMessage req, string username, TraceWriter log)
        {
            log.Info("OAuth Response");

            if (string.IsNullOrEmpty(username))
            {
                log.Error("Twitter/OAuth/{username} - Username is empty");
                return req.CreateResponse(HttpStatusCode.BadRequest, "Username was not supplied");
            }

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

            var myLogger = new TraceAdaptor(log);
            var twitter = new OAuthClient(myLogger);
            var twitterUser = twitter.GetUser(consumerKey, consumerSecret, oauthToken, oauthVerifier);

            var users = new UserUtilities(_storageProvider);
            var paprikaUser = users.GetUser(username);

            paprikaUser.TwitterId = twitterUser.UserId;
            paprikaUser.TwitterUsername = twitterUser.UserName;
            paprikaUser.OAuthToken = twitterUser.Token;
            paprikaUser.OAuthTokenSecret = twitterUser.TokenSecret;

            var result = await users.UpdateUserAsync(paprikaUser);

            if (result.Success)
            {
                // Redirect back to app
                var locationUri = new Uri(req.RequestUri.Host);
                log.Info("Redirecting to " + locationUri.ToString());

                HttpResponseMessage response = req.CreateResponse(HttpStatusCode.Moved);
                response.Headers.Location = locationUri;
                return response;
                //return req.CreateResponse(HttpStatusCode.OK, "All good, go to /api/Tweet/{username}");
            }
            else
            {
                return req.CreateResponse<string>(HttpStatusCode.InternalServerError, "Update user failed: " + result.Attachment);
            }
        }
    }
}
