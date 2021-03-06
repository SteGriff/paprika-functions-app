﻿using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<string> ReadGrammarAsync(string username)
        {
            var blobAccess = new BlobUtilities(_storageProvider);
            var blob = blobAccess.GetBlockBlob(username);

            var stream = await blob.OpenReadAsync(AccessCondition.GenerateEmptyCondition(), new BlobRequestOptions(), new OperationContext());

            var sr = new StreamReader(stream);

            return await sr.ReadToEndAsync();
        }
        
        public async Task<string> CopyGrammar(string fromUser, string toUser)
        {
            var Grammar = await ReadGrammarAsync(fromUser);
            WriteGrammar(toUser, Grammar);
            return Grammar;
        }
    }
}
