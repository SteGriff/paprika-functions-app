using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using PaprikaFunctionsApp.Common;
using PaprikaFunctionsApp.Common.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PaprikaFunctionsApp
{
    public static class TwitterDisconnect
    {
        private static AzureStorageProvider _storageProvider;

        [FunctionName("TwitterDisconnect")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "Twitter/Disconnect")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("TwitterDisconnect started");

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

            var users = new UserUtilities(_storageProvider);
            var user = users.GetUser(authorisation.Username);
            user.DisconnectTwitter();
            
            return req.CreateResponse(HttpStatusCode.OK, "Done");
        }
    }
}
