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

namespace PaprikaFunctionsApp
{
    public static class TwitterAuthorise
    {
        private static AzureStorageProvider _storageProvider;

        [FunctionName("TwitterAuthorise")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "Twitter/Authorise")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Start twitter auth");

            try
            {
                _storageProvider = StorageProvider.GetStorageProvider();
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Storage Connection Error");
            }

            //Check authentication and kick user with 401 if there's a problem
            var authorisation = new AuthenticationResponse();
            var authenticationStatus = authorisation.Get(_storageProvider, req);
            if (!authenticationStatus.Success)
            {
                return authenticationStatus.Attachment;
            }

            var consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            var consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
            var twitterCallback = ConfigurationManager.AppSettings["TwitterCallback"];
            
            if (!twitterCallback.EndsWith("/"))
            {
                twitterCallback += "/";
            }
            twitterCallback += authorisation.Username;
            var twitterCallbackUri = new Uri(twitterCallback);

            var myLogger = new TraceAdaptor(log);
            var twitter = new OAuthClient(myLogger);
            var location = twitter.GetAuthorizeUri(consumerKey, consumerSecret, twitterCallbackUri);

            HttpResponseMessage response = req.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = location;
            return response;
        }
    }
}
