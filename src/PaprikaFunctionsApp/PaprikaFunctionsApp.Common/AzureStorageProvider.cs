using Microsoft.WindowsAzure.Storage;

namespace PaprikaFunctionsApp.Common
{
    public class AzureStorageProvider
    {
        public string _connectionString;

        public AzureStorageProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public CloudStorageAccount StorageAccount
        {
            get
            {
                return CloudStorageAccount.Parse(_connectionString);
            }
        }
    }
}
