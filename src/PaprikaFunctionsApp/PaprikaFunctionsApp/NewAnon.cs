using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using PaprikaFunctionsApp.Common;
using System;
using PaprikaFunctionsApp.Common.Models;

namespace PaprikaFunctionsApp
{
    public static class NewAnon
    {
        private static AzureStorageProvider _storageProvider;

        [FunctionName("NewAnon")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "Anon/New")]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                _storageProvider = StorageProvider.GetStorageProvider();
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Storage Connection Error: " + ex.ToString());
            }

            var theName = new AnonymousCredentials();

            var userAccess = new UserUtilities(_storageProvider);
            var result = await userAccess.CreateUserAsync(theName.Name, theName.Password);

            if (!result.Success)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create anon user: " + result.Attachment);
            }

            var gramCache = new GrammarCache(_storageProvider);
            gramCache.CopyCache("default", theName.Name);

            return req.CreateResponse<BaseCredentials>(HttpStatusCode.Created, theName);
        }
    }
}
