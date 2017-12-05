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
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Grammar/Edit")]HttpRequestMessage req, TraceWriter log)
        {
            string username;
            if (req.Headers.Contains("username"))
            {
                username = req.Headers.GetValues("username").FirstOrDefault();
            }
            else
            { 
                return req.CreateResponse(HttpStatusCode.Unauthorized, "No username received");
            }

            string plainPasswordString;
            if (req.Headers.Contains("password"))
            {
                plainPasswordString = req.Headers.GetValues("password").FirstOrDefault();
            }
            else
            {
                return req.CreateResponse(HttpStatusCode.Unauthorized, "No password received");
            }

            //Get the user and check their auth
            var user = UserUtilities.GetUser(username);
            var encryptedPassword = Encryptor.Encrypt(plainPasswordString, user.Salt);
            var isAuthed = encryptedPassword == user.EncryptedPassword;
            if (!isAuthed)
            {
                return req.CreateResponse(HttpStatusCode.Unauthorized, "Incorrect username/password combination");
            }

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
                WriteGrammar(username, fileContent);
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Failed to write grammar");
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
