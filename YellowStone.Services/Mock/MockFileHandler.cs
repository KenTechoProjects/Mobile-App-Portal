using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using YellowStone.Services.Processors;

namespace YellowStone.Services.Mock
{
    public class MockFileHandler : IFileHandler
    {
        private readonly IOptions<AzureStorageSettings> _settings;
        CloudBlobClient _blobClient;
        public MockFileHandler(IOptions<AzureStorageSettings> settings)
        {
            _settings = settings;
            BuildStorageClient();
        }

        void BuildStorageClient()
        {
            CloudStorageAccount storageacc = CloudStorageAccount.Parse(_settings.Value.StorageConnectionString);
            _blobClient = storageacc.CreateCloudBlobClient();
        }
        public async Task DeleteFile(string fileName, string fileType)
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(_settings.Value.BlobContainerName);

            await container.CreateIfNotExistsAsync();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            await blockBlob.DeleteIfExistsAsync();

        }

        public async Task<byte[]> GetFile(string fileName, string fileType)
        {

            //The next 2 lines create if not exists a container named "democontainer"
            CloudBlobContainer container = _blobClient.GetContainerReference(_settings.Value.BlobContainerName);

            await container.CreateIfNotExistsAsync();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            byte[] fileBytes = new byte[blockBlob.StreamWriteSizeInBytes];
            await blockBlob.DownloadToByteArrayAsync(fileBytes, 0);
            return fileBytes;
        }

        public async Task UploadFile(IFormFile formFile, string fileName, string fileType)
        {
            var container = _blobClient.GetContainerReference(_settings.Value.BlobContainerName);

            await container.CreateIfNotExistsAsync();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            if (formFile.Length > 0)
            {
                using (var filestream = formFile.OpenReadStream())
                {
                    await blockBlob.UploadFromStreamAsync(filestream);
                }
            }
        }

        public Task ArchiveFile(string fileName, string fileType)
        {
            return Task.CompletedTask;
        }
    }
}
