using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Paprika.Net;
using System;
using Paprika.Net.Exceptions;

namespace PaprikaFunctionsApp
{
    public static class Resolve
    {
        [FunctionName("Resolve")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Resolve/{query}")]HttpRequestMessage req, string query, TraceWriter log)
        {
            log.Info(string.Format("Incoming Resolve, q='{0}'", query));

            var grammarLines = @"
* phrase
here are my [adjective] [things]
these [things] are all [adjective]
we want [things]! [adjective] [things]!

* adjective
tiny
inscrutable
excitable

* things
politicians
lawyers
kittens
particles
".Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var engine = new Core();
            engine.LoadGrammarFromString(grammarLines);

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
