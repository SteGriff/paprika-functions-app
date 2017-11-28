using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Text;
using System;

namespace PaprikaFunctionsApp
{
    public static class Load
    {
        private readonly static CloudStorageAccount _storageAccount =
            CloudStorageAccount.Parse("UseDevelopmentStorage=true");

        [FunctionName("Load")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Load")]HttpRequestMessage req, TraceWriter log)
        {
            MultipartMemoryStreamProvider stream;
            try
            {
                var multipartTask = req.Content.ReadAsMultipartAsync();
                if (!multipartTask.IsCompleted)
                {
                    multipartTask.RunSynchronously();
                }
                stream = multipartTask.Result;
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Posted data was not multipart or read failed");
            }

            string fileContent;
            try
            {
                var httpContent = stream.Contents[0];
                var contentTask = httpContent.ReadAsStringAsync();
                if (!contentTask.IsCompleted)
                {
                    contentTask.RunSynchronously();
                }
                fileContent = contentTask.Result;
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

        private static ICloudBlob GetBlockBlob(string username)
        {
            const string CONTAINER_NAME = "grammar";
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(CONTAINER_NAME);
            container.CreateIfNotExists();

            string filename = UserUtilities.GetFilenameForUser(username);
            ICloudBlob blob = container.GetBlockBlobReference(filename);
            return blob;
        }

        public static void WriteGrammar(string username, string grammar)
        {
            var blob = GetBlockBlob(username);

            var grammarBytes = Encoding.UTF8.GetBytes(grammar);
            var memStream = new MemoryStream(grammarBytes);
            blob.UploadFromStream(memStream);
        }

        public static string ReadGrammar(string username)
        {
            var blob = GetBlockBlob(username);

            var blobStream = blob.OpenRead();
            var tr = new StreamReader(blobStream);
            string grammarContent = tr.ReadToEnd();
            return grammarContent;
        }

        
    }
}
