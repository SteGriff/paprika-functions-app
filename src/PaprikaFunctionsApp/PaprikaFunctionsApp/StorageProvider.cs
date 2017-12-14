using PaprikaFunctionsApp.Common;
using System.Configuration;

namespace PaprikaFunctionsApp
{
    public class StorageProvider
    {
        public static AzureStorageProvider GetStorageProvider()
        {
            string connectionString = GetConnectionString();
            return new AzureStorageProvider(connectionString);
        }

        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["PrimaryStorage"].ConnectionString;
        }
    }
}
