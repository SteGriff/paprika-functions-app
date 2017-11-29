using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace PaprikaFunctionsApp.Common
{
    public static class BlobUtilities
    {
        public static ICloudBlob GetBlockBlob(string username)
        {
            const string CONTAINER_NAME = "grammar";
            CloudBlobClient blobClient = AzureStorageProvider.StorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(CONTAINER_NAME);
            container.CreateIfNotExistsAsync();

            string filename = UserUtilities.GetFilenameForUser(username);
            ICloudBlob blob = container.GetBlockBlobReference(filename);
            return blob;
        }
    }
}
