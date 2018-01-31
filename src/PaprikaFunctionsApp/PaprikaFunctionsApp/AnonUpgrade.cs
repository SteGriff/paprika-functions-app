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
    public static class AnonUpgrade
    {
        private static AzureStorageProvider _storageProvider;

        [FunctionName("AnonUpgrade")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "Anon/Upgrade")]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                _storageProvider = StorageProvider.GetStorageProvider();
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Storage Connection Error: " + ex.ToString());
            }

            //Check authentication and kick user with 401 if there's a problem
            var authorisation = new AuthenticationResponse();
            var authenticationStatus = authorisation.Get(_storageProvider, req);
            if (!authenticationStatus.Success)
            {
                return authenticationStatus.Attachment;
            }

            // Get new details from request body 
            dynamic data = await req.Content.ReadAsAsync<object>();
            string newUsername = data?.newUsername;
            string newPassword = data?.newPassword;

            var users = new UserUtilities(_storageProvider);
            var renameUserResult = await users.RenameUserAsync(authorisation.Username, newUsername, newPassword);

            return req.CreateResponse<string>(HttpStatusCode.Created, "OK");
        }
    }
}