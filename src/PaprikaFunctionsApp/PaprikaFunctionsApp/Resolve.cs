using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Paprika.Net;
using System;
using Paprika.Net.Exceptions;
using PaprikaFunctionsApp.Common;

namespace PaprikaFunctionsApp
{
    public static class Resolve
    {
        private static AzureStorageProvider _storageProvider;

        [FunctionName("Resolve")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Grammar/Resolve/{query}")]HttpRequestMessage req, string query, TraceWriter log)
        {
            log.Info(string.Format("Incoming Resolve, q='{0}'", query));

            try
            {
                _storageProvider = StorageProvider.GetStorageProvider();
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Storage Connection Error");
            }

            //Check authentication and kick user with 401 if there's a problem
            var authChecker = new AuthenticationResponse();
            var authenticationStatus = authChecker.Get(_storageProvider, req);
            if (!authenticationStatus.Success)
            {
                return authenticationStatus.Attachment;
            }
            
            // Get user's grammar (kick them if it doesn't exist)
            var cache = new GrammarCache(_storageProvider);
            var grammarObject = cache.ReadFromCache(authChecker.Username);
            if (grammarObject.GrammarDictionary == null)
            {
                return req.CreateResponse(HttpStatusCode.Conflict, "No grammar loaded for user");
            }

            //Try to load the grammar
            Core engine;
            try
            {
                engine = new Core();
                engine.LoadThisGrammar(grammarObject.GrammarDictionary);
            }
            catch (Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Failed to load grammar: " + ex.Message);
            }

            //Parse the req and return the answer or Paprika's error message otherwise
            try
            {
                string answer = engine.Parse(query);
                log.Info("Reponse=" + answer);
                return req.CreateResponse(HttpStatusCode.OK, answer, "text/plain");
            }
            catch (PaprikaException ex)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Paprika error: " + ex.Message);
            }
        }
    }
}
