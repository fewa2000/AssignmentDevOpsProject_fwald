using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AssignmentDevOpsProject_fwald.Services
{
    public class BlobStorage
    {
        private readonly string _storageConnectionString;

        public BlobStorage(string storageConnectionString)
        {
            _storageConnectionString = storageConnectionString;
        }

        public async Task UploadImageAsync(Stream imageStream, string blobContainerName, string fileName)
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(_storageConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);

                // Create the container if it does not exist.
                await blobContainerClient.CreateIfNotExistsAsync();

                var blobClient = blobContainerClient.GetBlobClient(fileName);

                // Reset stream position
                imageStream.Position = 0;

                // Upload image stream to blob storage
                await blobClient.UploadAsync(imageStream, overwrite: true);
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine($"Error uploading to Blob Storage: {ex.Message}");
                throw;
            }
        }
    }
}
