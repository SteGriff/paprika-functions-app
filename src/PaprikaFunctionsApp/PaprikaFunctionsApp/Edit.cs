using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
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
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "/Grammar/Edit")]HttpRequestMessage req, TraceWriter log)
        {
            string username = req.Headers.GetValues("username").FirstOrDefault();
            if (string.IsNullOrEmpty(username))
            {
                return req.CreateResponse(HttpStatusCode.Unauthorized, "Bad username");
            }

            string password = req.Headers.GetValues("password").FirstOrDefault();
            if (string.IsNullOrEmpty(password))
            {
                return req.CreateResponse(HttpStatusCode.Unauthorized, "Bad username");
            }

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
                WriteGrammar("stegriff", fileContent);
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Failed to write grammar");
            }

            return req.CreateResponse(HttpStatusCode.Created, "Hello world");
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
