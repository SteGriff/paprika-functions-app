using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace PaprikaFunctionsApp.Common
{
    public static class BlobUtilities
    {
        private readonly static CloudStorageAccount _storageAccount =
            CloudStorageAccount.Parse("UseDevelopmentStorage=true");

        public static ICloudBlob GetBlockBlob(string username)
        {
            const string CONTAINER_NAME = "grammar";
            CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(CONTAINER_NAME);
            container.CreateIfNotExistsAsync();

            string filename = UserUtilities.GetFilenameForUser(username);
            ICloudBlob blob = container.GetBlockBlobReference(filename);
            return blob;
        }
    }
}
