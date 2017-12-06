using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using PaprikaFunctionsApp.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaprikaFunctionsApp.Common.Models
{
    public class GrammarEntity : TableEntity
    {
        public GrammarModel GrammarObject { get; set; }
        public string GrammarJson { get; set; }
        public string Username { get; set; }
        public DateTime CreatedDate { get; set; }

        public GrammarEntity(GrammarModel grammar, string username, DateTime created)
        {
            PartitionKey = username;
            RowKey = created.ToIso8601();

            GrammarObject = grammar;
            GrammarJson = JsonConvert.SerializeObject(grammar);
            Username = username;
            CreatedDate = created;
        }

        public GrammarEntity() { }

    }
}
