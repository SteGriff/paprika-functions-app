using Microsoft.WindowsAzure.Storage.Table;
using PaprikaFunctionsApp.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaprikaFunctionsApp.Common.Models
{
    public class GrammarEntity : TableEntity
    {
        public Dictionary<string, List<string>> Grammar { get; set; }
        public string Username { get; set; }
        public DateTime CreatedDate { get; set; }

        public GrammarEntity(Dictionary<string, List<string>> grammar, string username, DateTime created)
        {
            PartitionKey = username;
            RowKey = created.ToIso8601();

            Grammar = grammar;
            Username = username;
            CreatedDate = created;
        }

        public GrammarEntity() { }

    }
}
