using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaprikaFunctionsApp.Common
{
    public class SampleData
    {
        public string Grammar { get; set; }
        private AzureStorageProvider _storageProvider;
        private GrammarBlob _grammarBlob;

        public SampleData(AzureStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
            _grammarBlob = new GrammarBlob(_storageProvider);
        }

        public async Task<string> PopulateAsync(string username)
        {
            //Copy the default grammar to storage for this user
            Grammar = await _grammarBlob.ReadGrammarAsync("default");
            _grammarBlob.WriteGrammar(username, Grammar);
            return Grammar;
        }

    }
}
