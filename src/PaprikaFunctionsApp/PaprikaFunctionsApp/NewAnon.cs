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
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Storage Connection Error");
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
                var userCreationResult = await userAccess.CreateUserAsync(theName.Name, theName.Password, true);

                if (!userCreationResult.Success)
                {
                    return req.CreateResponse(HttpStatusCode.InternalServerError, "Failed to create anon user: " + userCreationResult.Attachment);
                }
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "User creation failed: " + ex.ToString());
            }

            const string DEFAULT_USER = "default";

            try
            {
                //Copy the default grammar to storage for this user
                var blob = new GrammarBlob(_storageProvider);
                await blob.CopyGrammar(DEFAULT_USER, theName.Name);
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Blob copy failed for new user - admin should re-run migrations");
            }

            try
            {
                // Copy grammar cache
                var gramCache = new GrammarCache(_storageProvider);
                gramCache.CopyCache(DEFAULT_USER, theName.Name);
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Cache copy failed for new user - admin should re-run migrations");
            }

            return req.CreateResponse<BaseCredentials>(HttpStatusCode.Created, theName);
        }
    }
}
