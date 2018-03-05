using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.IO;
using System.Net;
using System.Net.Http;

namespace PaprikaFunctionsApp
{
    public static class LetsEncrypt
    {
        [FunctionName("LetsEncrypt")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "LetsEncrypt/{code}")]HttpRequestMessage req, string code, TraceWriter log)
        {
            var content = File.ReadAllText(@"D:\home\site\wwwroot\.well-known\acme-challenge\" + code);
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent(content, System.Text.Encoding.UTF8, "text/plain");
            return resp;
        }
    }
}
