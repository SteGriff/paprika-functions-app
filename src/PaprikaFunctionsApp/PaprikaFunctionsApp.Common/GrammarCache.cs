using Microsoft.WindowsAzure.Storage.Table;
using PaprikaFunctionsApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Collections;

namespace PaprikaFunctionsApp.Common
{
    public class GrammarCache
    {
        private AzureStorageProvider _storageProvider;
        private TableUtilities _tableAccess;
        const string GRAMMAR = "grammar";

        public GrammarCache(AzureStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
            _tableAccess = new TableUtilities(_storageProvider);
        }

        public Status<object> WriteToCache(GrammarModel grammar, string username, DateTime created)
        {
            var table = _tableAccess.GetTable(GRAMMAR);
            var newGrammar = new GrammarEntity(grammar, username, created);
            var insert = TableOperation.Insert(newGrammar);
            var result = table.ExecuteAsync(insert).Result;
            if (result != null)
            {
                return new Status<object>(true, result.Result);
            }
            return new Status<object>(false);
        }

        private GrammarEntity GetCurrentGrammarForUser(string username, out CloudTable table)
        {
            table = _tableAccess.GetTable(GRAMMAR);
            var query = new TableQuery<GrammarEntity>()
            {
                FilterString = TableQuery.GenerateFilterCondition("PartitionKey", "eq", username)
            };
            var results = table.ExecuteQuerySegmentedAsync(query, new TableContinuationToken()).Result;
            var latestGrammar = results.Results.OrderByDescending(g => g.RowKey).FirstOrDefault();
            return latestGrammar;
        }

        public GrammarModel ReadFromCache(string username)
        {
            try
            {
                CloudTable table;
                var latestGrammar = GetCurrentGrammarForUser(username, out table);
                var grammarObject = JsonConvert.DeserializeObject<GrammarModel>(latestGrammar.GrammarJson);
                return grammarObject;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Status<string> CopyCache(string fromUser, string toUser)
        {
            try
            {
                CloudTable table;
                var latestGrammar = GetCurrentGrammarForUser(fromUser, out table);

                //Copy to new user
                latestGrammar.PartitionKey = toUser;

                var insert = TableOperation.Insert(latestGrammar);
                var result = table.ExecuteAsync(insert).Result;
            }
            catch (Exception ex)
            {
                return new Status<string>(false, "CopyCache failed: " + ex.ToString());
            }
            
            return new Status<string>(true);
        }
    }
}
