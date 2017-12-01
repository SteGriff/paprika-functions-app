using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaprikaFunctionsApp.Common
{
    public static class TableUtilities
    {
        public static CloudTable GetTable(string tablename)
        {
            CloudTableClient tableClient = AzureStorageProvider.StorageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tablename);
            table.CreateIfNotExistsAsync();
            
            return table;
        }

        public static async Task DropTableAsync(string tablename)
        {
            CloudTableClient tableClient = AzureStorageProvider.StorageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tablename);
            await table.DeleteIfExistsAsync();
        }
    }
}
