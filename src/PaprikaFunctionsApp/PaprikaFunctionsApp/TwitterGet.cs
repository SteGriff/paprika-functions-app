using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using PaprikaFunctionsApp.Common.Models;
using PaprikaFunctionsApp.Common;
using System;

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
            
            return req.CreateResponse<UserTwitterViewModel>(HttpStatusCode.OK, userTwitterModel, "application/json");
        }
    }
}
