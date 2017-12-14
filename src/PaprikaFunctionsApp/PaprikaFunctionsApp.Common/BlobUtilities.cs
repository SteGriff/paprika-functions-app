using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Text;

namespace PaprikaFunctionsApp.Common
{
    public class BlobUtilities
    {
        private AzureStorageProvider _storageProvider;

        public BlobUtilities(AzureStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public ICloudBlob GetBlockBlob(string username)
        {
            const string CONTAINER_NAME = "grammar";
            CloudBlobClient blobClient = _storageProvider.StorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(CONTAINER_NAME);
            container.CreateIfNotExistsAsync();

            var userUtils = new UserUtilities(_storageProvider);
            string filename = userUtils.GetFilenameForUser(username);
            ICloudBlob blob = container.GetBlockBlobReference(filename);
            return blob;
        }
    }
}
