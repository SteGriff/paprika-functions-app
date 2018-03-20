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

namespace PaprikaFunctionsApp
{
    public static class TwitterAuthorise
    {
        [FunctionName("TwitterAuthorise")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Twitter/Authorise")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Start twitter auth");

            var consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            var consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
            var twitterCallback = ConfigurationManager.AppSettings["TwitterCallback"];

            var myLogger = new TraceAdaptor(log);
            var twitter = new OAuthClient(myLogger);
            var location = twitter.GetAuthorizeUri(consumerKey, consumerSecret, new Uri(twitterCallback));

            HttpResponseMessage response = req.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = location;
            return response;
        }
    }
}
