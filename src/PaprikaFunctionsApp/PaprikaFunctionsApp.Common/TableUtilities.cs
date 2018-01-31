using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaprikaFunctionsApp.Common
{
    public class TableUtilities
    {
        private AzureStorageProvider _storageProvider;

        public TableUtilities(AzureStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public CloudTable GetTable(string tablename)
        {
            CloudTableClient tableClient = _storageProvider.StorageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tablename);
            table.CreateIfNotExistsAsync().Wait();
            
            return table;
        }

        public async Task DropTableAsync(string tablename)
        {
            CloudTableClient tableClient = _storageProvider.StorageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tablename);
            await table.DeleteIfExistsAsync();
        }
    }
}
