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
    public static class TwitterSet
    {
        private static AzureStorageProvider _storageProvider;

        [FunctionName("TwitterSet")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "Twitter/Set")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Started TwitterSet");

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

            log.Info("Get info from request body...");

            // Get new details from request body 
            UserTwitterViewModel data;
            try
            {
                data = await req.Content.ReadAsAsync<UserTwitterViewModel>();
                log.Info(data.ToString());
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Failed to decode UserTwitterViewModel: " + ex.Message);
            }

            log.Info("Get user...");
            var users = new UserUtilities(_storageProvider);
            var user = users.GetUser(authorisation.Username);

            log.Info("Merge data...");
            user.MergeFromTwitterModel(data);

            log.Info("Update user...");
            var result = await users.UpdateUserAsync(user);
            if (!result.Success)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, result.Attachment);
            }

            log.Info("Success!");
            return req.CreateResponse(HttpStatusCode.OK, "Schedule updated");
        }
    }
}
