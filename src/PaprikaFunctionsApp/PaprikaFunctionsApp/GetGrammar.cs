using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using PaprikaFunctionsApp.Common;

namespace PaprikaFunctionsApp
{
    public static class GetGrammar
    {
        private static AzureStorageProvider _storageProvider;

        [FunctionName("GetGrammar")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "Grammar/GetGrammar")]HttpRequestMessage req, TraceWriter log)
        {
            try
            {
                _storageProvider = StorageProvider.GetStorageProvider();
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Storage Connection Error");
            }

            //Check authentication and kick user with 401 if there's a problem
            var authResponse = new AuthenticationResponse();
            var authenticationStatus = authResponse.Get(_storageProvider, req);
            if (!authenticationStatus.Success)
            {
                return authenticationStatus.Attachment;
            }

            var grammarAccess = new GrammarBlob(_storageProvider);

            var grammarString = await grammarAccess.ReadGrammarAsync(authResponse.Username);
            return req.CreateResponse(grammarString);
        }
    }
}
