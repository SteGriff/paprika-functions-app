using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using PaprikaFunctionsApp.Common;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;

namespace PaprikaFunctionsApp
{
    public static class UploadFile
    {
        private static AzureStorageProvider _storageProvider;

        [FunctionName("UploadFile")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "Grammar/UploadFile")]HttpRequestMessage req, TraceWriter log)
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

            //Authenticated user:-
            //Get the posted file and decode it
            MultipartMemoryStreamProvider stream;
            try
            {
                stream = req.Content.ReadAsMultipartAsync().Result;
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Posted data was not multipart or read failed");
            }

            string fileContent;
            try
            {
                var httpContent = stream.Contents[0];
                fileContent = httpContent.ReadAsStringAsync().Result;
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
                var storageProvider = StorageProvider.GetStorageProvider();
                var grammarBlob = new GrammarBlob(storageProvider);
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
