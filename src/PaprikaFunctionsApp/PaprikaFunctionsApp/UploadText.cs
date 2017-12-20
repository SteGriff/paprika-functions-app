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
    public static class UploadText
    {
        private static AzureStorageProvider _storageProvider;

        [FunctionName("UploadText")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "Grammar/UploadText")]HttpRequestMessage req, TraceWriter log)
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
            var authResponse = new AuthenticationResponse();
            var authenticationStatus = authResponse.Get(_storageProvider, req);
            if (!authenticationStatus.Success)
            {
                return authenticationStatus.Attachment;
            }

            //Authenticated user:-
            //Get the uploaded content
            string fileContent;
            try
            {
                // Get POST body
                fileContent = await req.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Unable to read file stream");
            }

            var parseAndCacheResponse = new ParseAndCacheResponse();
            var parseAndCacheStatus = parseAndCacheResponse.Get(fileContent, authResponse.Username, _storageProvider, req);
            if (!parseAndCacheStatus.Success)
            {
                return parseAndCacheStatus.Attachment;
            }

            try
            {
                var grammarBlob = new GrammarBlob(_storageProvider);
                grammarBlob.WriteGrammar(authResponse.Username, fileContent);
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Failed to write grammar");
            }

            return req.CreateResponse(HttpStatusCode.Created, "Saved");
        }
    }
}
