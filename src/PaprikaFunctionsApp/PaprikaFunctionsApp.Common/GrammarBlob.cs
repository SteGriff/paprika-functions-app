using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PaprikaFunctionsApp.Common
{
    public static class GrammarBlob
    {
        public static void WriteGrammar(string username, string grammar)
        {
            var blob = BlobUtilities.GetBlockBlob(username);

            var grammarBytes = Encoding.UTF8.GetBytes(grammar);
            var memStream = new MemoryStream(grammarBytes);
            blob.UploadFromStreamAsync(memStream).Wait();
        }
    }
}
