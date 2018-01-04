using Microsoft.Azure.WebJobs;
using Paprika.Net;
using PaprikaFunctionsApp.Common;
using PaprikaFunctionsApp.Common.Behaviour;
using PaprikaFunctionsApp.Common.Extensions;
using PaprikaFunctionsApp.Common.Models;
using System;
using System.Net;
using System.Net.Http;

namespace PaprikaFunctionsApp
{
    public class ParseAndCacheResponse
    {
        public Status<HttpResponseMessage> Get(string fileContent, string username, AzureStorageProvider storageProvider, [HttpTrigger]HttpRequestMessage req)
        {
            var parseAndCacheAction = new ParseAndCache();
            var result = parseAndCacheAction.Run(fileContent, username, storageProvider);
            if (result.Success)
            {
                return new Status<HttpResponseMessage>(true);
            }
            else
            {
                var statusCode = result.Attachment.GetHttpStatusCode();
                return new Status<HttpResponseMessage>(false, req.CreateResponse(statusCode, result.Attachment.Message));
            }
        }

    }
}
