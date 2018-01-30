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

            AnonymousCredentials theName;
            try
            {
                theName = new AnonymousCredentials();
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Name generation failed: " + ex.ToString());
            }

            try
            {
                var userAccess = new UserUtilities(_storageProvider);
                var userCreationResult = await userAccess.CreateUserAsync(theName.Name, theName.Password);

                if (!userCreationResult.Success)
                {
                    return req.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create anon user: " + userCreationResult.Attachment);
                }
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "User creation failed: " + ex.ToString());
            }

            try
            {
                // Copy grammar blob
                var sampleData = new SampleData(_storageProvider);
                await sampleData.PopulateAsync(theName.Name);
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Blob copy failed for new user - admin should re-run migrations");
            }

            try
            {
                // Copy grammar cache
                var gramCache = new GrammarCache(_storageProvider);
                gramCache.CopyCache("default", theName.Name);
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Cache copy failed for new user - admin should re-run migrations");
            }

            return req.CreateResponse<BaseCredentials>(HttpStatusCode.Created, theName);
        }
    }
}
