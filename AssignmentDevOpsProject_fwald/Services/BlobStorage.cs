using Azure.Storage.Blobs;
using Azure.Storage.Sas;
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

        public async Task<string> UploadImageAsync(Stream imageStream, string blobContainerName, string fileName)
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(_storageConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);

                await blobContainerClient.CreateIfNotExistsAsync();

                var blobClient = blobContainerClient.GetBlobClient(fileName);

                imageStream.Position = 0;
                await blobClient.UploadAsync(imageStream, overwrite: true);

                // Generate SAS token for the blob
                string sasToken = GenerateBlobSasToken(blobContainerClient, fileName);
                return blobClient.Uri + sasToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading to Blob Storage: {ex.Message}");
                throw;
            }
        }

        private string GenerateBlobSasToken(BlobContainerClient containerClient, string blobName)
        {
            var blobClient = containerClient.GetBlobClient(blobName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerClient.Name,
                BlobName = blobName,
                Resource = "b", // b for blob
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(24) // 24-hour valid token
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read); // Read permissions

            var sasToken = blobClient.GenerateSasUri(sasBuilder).Query;
            return sasToken;
        }
    }
}
