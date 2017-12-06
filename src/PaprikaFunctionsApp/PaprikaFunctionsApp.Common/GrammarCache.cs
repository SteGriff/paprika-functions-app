using Microsoft.WindowsAzure.Storage.Table;
using PaprikaFunctionsApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PaprikaFunctionsApp.Common
{
    public class GrammarCache
    {
        public Status<object> WriteToCache(Dictionary<string, List<string>> grammar, string username, DateTime created)
        {
            var table = TableUtilities.GetTable("grammar");
            var newGrammar = new GrammarEntity(grammar, username, created);
            var insert = TableOperation.Insert(newGrammar);
            var result = table.ExecuteAsync(insert).Result;
            if (result != null)
            {
                return new Status<object>(result.Result, true);
            }
            return new Status<object>(false);
        }

        public Dictionary<string, List<string>> ReadFromCache(string username)
        {
            var table = TableUtilities.GetTable("grammar");
            var query = new TableQuery<GrammarEntity>() { FilterString = TableQuery.GenerateFilterCondition("PartitionKey", "eq", username) };
            var results = table.ExecuteQuerySegmentedAsync(query, new TableContinuationToken()).Result;
            var latestGrammar = results.Results.OrderByDescending(g => g.RowKey).FirstOrDefault();
            return latestGrammar.Grammar;
        }
    }
}
