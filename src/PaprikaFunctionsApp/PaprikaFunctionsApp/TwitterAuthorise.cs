using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Configuration;
using PaprikaFunctionsApp.Common.Twitter;
using System;
using PaprikaFunctionsApp.Adaptors;
using PaprikaFunctionsApp.Common;
using PaprikaFunctionsApp.Common.Extensions;

namespace PaprikaFunctionsApp
{
    public static class TwitterAuthorise
    {
        private static AzureStorageProvider _storageProvider;

        [FunctionName("TwitterAuthorise")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Twitter/Authorise/{identifier}")]HttpRequestMessage req, string identifier, TraceWriter log)
        {
            log.Info("Start TwitterAuthorise with id:" + identifier);

            if (string.IsNullOrEmpty(identifier))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Bad identifier");
            }

            try
            {
                _storageProvider = StorageProvider.GetStorageProvider();
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Storage Connection Error");
            }

            //Change {identifier} into authentication headers
            req.AddAuthenticationHeadersFromIdentifier(identifier);

            //Check authentication and kick user with 401 if there's a problem
            var authorisation = new AuthenticationResponse();
            var authenticationStatus = authorisation.Get(_storageProvider, req);
            if (!authenticationStatus.Success)
            {
                return authenticationStatus.Attachment;
            }

            string consumerKey;
            string consumerSecret;
            string twitterCallback;
            try
            {
                consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
                consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
                twitterCallback = ConfigurationManager.AppSettings["TwitterCallback"];
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "App is configured wrong; missing twitter API details");
            }

            if (!twitterCallback.EndsWith("/"))
            {
                twitterCallback += "/";
            }
            twitterCallback += authorisation.Username;
            var twitterCallbackUri = new Uri(twitterCallback);

            var myLogger = new TraceAdaptor(log);
            var twitter = new OAuthClient(myLogger);
            var location = twitter.GetAuthorizeUri(consumerKey, consumerSecret, twitterCallbackUri);

            log.Info("Redirecting to " + location.ToString());

            HttpResponseMessage response = req.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = location;
            return response;
        }
    }
}
