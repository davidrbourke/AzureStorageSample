using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;

namespace AzureStorage
{
    public class StorageAccountBlob
    {
        private CloudStorageAccount _storageAccount;
        private CloudBlobContainer _container;

        public StorageAccountBlob()
        {
            // Sample run
            CreateAccount();
            UploadAFile("img2.jpg", @"C:\Users\David\Pictures\img2.jpg");
            PrintAllContrainers();
            CopyAFile("img2.jpg", "img3.jpg");
            DowloadAFile("img3.jpg", @"C:\Users\David\Pictures\Dwn\img3.jpg");
            SetMetaData(_container);
            GetMetaData(_container);
        }

        public void CreateAccount()
        {
            _storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnection"));

            var blobClient = _storageAccount.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference("images");

            _container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);
        }

        public void UploadAFile(string fileName, string source)
        {
            var blockBlob = _container.GetBlockBlobReference(fileName);
            // Use a prefix for 'folder' structure
            // var blockBlobCopyWithPrefix = _container.GetBlockBlobReference("jpg-images/img5.jpg");
            using (var fileStream = File.OpenRead(source))
            {
                blockBlob.UploadFromStream(fileStream);
            }
        }

        public void PrintAllContrainers()
        {
            var blobs = _container.ListBlobs();
            foreach (var blob in blobs)
            {
                Console.WriteLine(blob.Uri);
            }
        }

        public void DowloadAFile(string fileName, string destination)
        {
            var blockBlob = _container.GetBlockBlobReference(fileName);
            using (var fileStream = File.Open(destination, FileMode.Create, FileAccess.Write))
            {
                blockBlob.DownloadToStream(fileStream);
            }
        }

        public void CopyAFile(string sourceFileName, string targetFileName)
        {
            var blockBlob = _container.GetBlockBlobReference(sourceFileName);
            var blockBlobCopy = _container.GetBlockBlobReference(targetFileName);
            var cb = new AsyncCallback(x => Console.WriteLine("blob copy completed"));

            blockBlobCopy.BeginStartCopy(blockBlob, cb, null);
        }

        public void SetMetaData(CloudBlobContainer container)
        {
            container.Metadata.Clear();
            container.Metadata.Add("Owner", "David");
            container.Metadata["Updated"] = DateTime.Now.ToString();
            container.SetMetadata();
        }

        public void GetMetaData(CloudBlobContainer container)
        {
            container.FetchAttributes();
            Console.WriteLine("Container metadata: \n");
            foreach (var item in container.Metadata)
            {
                Console.WriteLine(
                    string.Format($"{item.Key} {item.Value}"));
            }
        }
    }
}
