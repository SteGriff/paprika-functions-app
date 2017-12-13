using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace PaprikaFunctionsApp
{
    public static class UploadText
    {
        [FunctionName("UploadText")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "Grammar/UploadText")]HttpRequestMessage req, TraceWriter log)
        {
            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            string text = data?.text;

            return text == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "No text")
                : req.CreateResponse(HttpStatusCode.OK, "Got '" + text + "'");
        }
    }
}
