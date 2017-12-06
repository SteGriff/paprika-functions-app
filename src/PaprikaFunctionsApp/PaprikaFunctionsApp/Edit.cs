using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Paprika.Net;
using PaprikaFunctionsApp.Common;
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

        [FunctionName("Edit")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Grammar/Edit")]HttpRequestMessage req, TraceWriter log)
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

            Core engine;
            try
            {
                var grammarLines = fileContent.Split(new char[] { '\n' });
                engine = new Core();
                engine.LoadGrammarFromString(grammarLines);
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Failed to parse grammar: " + ex.Message);
            }

            try
            {
                var gramCache = new GrammarCache();
                gramCache.WriteToCache(engine.Grammar, authChecker.Username, DateTime.Now);
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Failed to cache grammar: " + ex.Message);
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

        public static string ReadGrammar(string username)
        {
            var blob = BlobUtilities.GetBlockBlob(username);

            var blobStream = blob.OpenRead();
            var tr = new StreamReader(blobStream);
            string grammarContent = tr.ReadToEnd();
            return grammarContent;
        }
        
    }
}
