using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Paprika.Net;
using PaprikaFunctionsApp.Common;
using PaprikaFunctionsApp.Common.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace PaprikaFunctionsApp
{
    public static class Edit
    {

        [FunctionName("UploadFile")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Grammar/UploadFile")]HttpRequestMessage req, TraceWriter log)
        {
            //Check authentication and kick user with 401 if there's a problem
            var authChecker = new Authenticator();
            var authenticationStatus = authChecker.IsAuthorised(req);
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

            try
            {
                WriteGrammar(authChecker.Username, fileContent);
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Failed to write grammar");
            }

            var parseAndCacheStatus = ParseAndCache(fileContent, authChecker.Username, req);
            if (!parseAndCacheStatus.Success)
            {
                return parseAndCacheStatus.Attachment;
            }

            return req.CreateResponse(HttpStatusCode.Created, "Saved");
        }

        public static void WriteGrammar(string username, string grammar)
        {
            var blob = BlobUtilities.GetBlockBlob(username);

            var grammarBytes = Encoding.UTF8.GetBytes(grammar);
            var memStream = new MemoryStream(grammarBytes);
            blob.UploadFromStream(memStream);
        }

        public static Status<HttpResponseMessage> ParseAndCache(string fileContent, string username, [HttpTrigger]HttpRequestMessage req)
        {
            Core engine;
            try
            {
                var grammarLines = fileContent.Split(new char[] { '\n' });
                engine = new Core();
                engine.LoadGrammarFromString(grammarLines);
            }
            catch (Exception ex)
            {
                return new Status<HttpResponseMessage>(req.CreateResponse(HttpStatusCode.InternalServerError, "Failed to parse grammar: " + ex.Message), false);
            }

            try
            {
                var gramCache = new GrammarCache();
                var grammarModel = new GrammarModel(engine.Grammar);
                gramCache.WriteToCache(grammarModel, username, DateTime.Now);
            }
            catch (Exception ex)
            {
                return new Status<HttpResponseMessage>(req.CreateResponse(HttpStatusCode.InternalServerError, "Failed to cache grammar: " + ex.Message), false);
            }

            return new Status<HttpResponseMessage>(true);
        }

    }
}
