using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using PaprikaFunctionsApp.Common;
using PaprikaFunctionsApp.Common.Models;
using System;
using System.Net;
using System.Net.Http;

namespace PaprikaFunctionsApp
{
    public static class TwitterGet
    {
        private static AzureStorageProvider _storageProvider;

        [FunctionName("TwitterGet")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Twitter/Get")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("TwitterGet started");

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
            var userTwitterModel = new UserTwitterViewModel(user);
            
            if (!string.IsNullOrEmpty(userTwitterModel.TwitterUsername))
            {
                return req.CreateResponse<UserTwitterViewModel>(HttpStatusCode.OK, userTwitterModel, "application/json");
            }
            else
            {
                //Return a null response if username is not set
                return req.CreateResponse<UserTwitterViewModel>(HttpStatusCode.OK, null, "application/json");
            }
        }
    }
}
