using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaprikaFunctionsApp.Common
{
    static class AzureStorageProvider
    {
        static string connectionString = "UseDevelopmentStorage=true";

        public static CloudStorageAccount StorageAccount
        {
            get
            {
                return CloudStorageAccount.Parse(connectionString);
            }
        }
    }
}
