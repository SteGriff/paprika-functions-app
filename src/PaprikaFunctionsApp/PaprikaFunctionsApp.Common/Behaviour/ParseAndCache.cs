using Paprika.Net;
using PaprikaFunctionsApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace PaprikaFunctionsApp.Common.Behaviour
{
    public class ParseAndCache
    {
        public Status<CodeAndMessage> Run(string fileContent, string username, AzureStorageProvider storageProvider)
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
                return new Status<CodeAndMessage>(false, new CodeAndMessage(500, "Failed to parse grammar: " + ex.Message));
            }

            try
            {
                var gramCache = new GrammarCache(storageProvider);
                var grammarModel = new GrammarModel(engine.Grammar);
                gramCache.WriteToCache(grammarModel, username, DateTime.Now);
            }
            catch (Exception ex)
            {
                return new Status<CodeAndMessage>(false, new CodeAndMessage(500, "Failed to cache grammar: " + ex.Message));
            }

            return new Status<CodeAndMessage>(true);
        }

    }
}
