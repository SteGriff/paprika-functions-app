using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PaprikaFunctionsApp.Common
{
    public class GrammarBlob
    {
        private AzureStorageProvider _storageProvider;

        public GrammarBlob(AzureStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public void WriteGrammar(string username, string grammar)
        {
            var blobAccess = new BlobUtilities(_storageProvider);
            var blob = blobAccess.GetBlockBlob(username);

            var grammarBytes = Encoding.UTF8.GetBytes(grammar);
            var memStream = new MemoryStream(grammarBytes);
            blob.UploadFromStreamAsync(memStream).Wait();
        }
    }
}
