using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaprikaFunctionsApp.Common
{
    public static class TableUtilities
    {
        public static CloudTable GetTable(string tablename)
        {
            CloudTableClient tableClient = AzureStorageProvider.StorageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tablename);
            table.CreateIfNotExistsAsync();

            //var query = new TableQuery()
            //{ FilterString = "" };

            //table.ExecuteQuerySegmentedAsync(query);

            return table;
        }
    }
}
