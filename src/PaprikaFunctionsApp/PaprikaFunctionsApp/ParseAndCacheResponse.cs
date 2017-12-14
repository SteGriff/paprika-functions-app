﻿using Microsoft.Azure.WebJobs;
using Paprika.Net;
using PaprikaFunctionsApp.Common;
using PaprikaFunctionsApp.Common.Models;
using System;
using System.Net;
using System.Net.Http;

namespace PaprikaFunctionsApp
{
    public class ParseAndCacheResponse
    {
        public Status<HttpResponseMessage> Get(string fileContent, string username, [HttpTrigger]HttpRequestMessage req)
        {
            Core engine;
            try
            {
                var grammarLines = fileContent.Split(new char[] { '\n' });
                engine = new Core();
                engine.LoadGrammarFromString(grammarLines);
            }
            catch (Exception ex)
            {
                return new Status<HttpResponseMessage>(req.CreateResponse(HttpStatusCode.InternalServerError, "Failed to parse grammar: " + ex.Message), false);
            }

            try
            {
                var gramCache = new GrammarCache();
                var grammarModel = new GrammarModel(engine.Grammar);
                gramCache.WriteToCache(grammarModel, username, DateTime.Now);
            }
            catch (Exception ex)
            {
                return new Status<HttpResponseMessage>(req.CreateResponse(HttpStatusCode.InternalServerError, "Failed to cache grammar: " + ex.Message), false);
            }

            return new Status<HttpResponseMessage>(true);
        }

    }
}