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
        [FunctionName("Resolve")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Grammar/Resolve/{query}")]HttpRequestMessage req, string query, TraceWriter log)
        {
            log.Info(string.Format("Incoming Resolve, q='{0}'", query));

            //Check authentication and kick user with 401 if there's a problem
            var authChecker = new Authenticator();
            var authenticationStatus = authChecker.IsAuthorised(req);
            if (!authenticationStatus.Success)
            {
                return authenticationStatus.Attachment;
            }

            var cache = new GrammarCache();
            var grammarObject = cache.ReadFromCache(authChecker.Username).ToPaprikaDictionary();

            var engine = new Core();
            engine.LoadThisGrammar(grammarObject);

            string answer = "";
            try
            {
                answer = engine.Parse(query);
                log.Info("Reponse=" + answer);
            }
            catch (PaprikaException ex)
            {
                answer = ex.ToString();
            }

            return req.CreateResponse(HttpStatusCode.OK, answer, "text/plain");

        }
    }
}
