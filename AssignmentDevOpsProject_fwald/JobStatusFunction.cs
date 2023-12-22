using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AssignmentDevOpsProject_fwald
{
    public static class JobStatusFunction
    {
        [FunctionName("JobStatusFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "jobstatus/{jobId}")] HttpRequest req,
            string jobId,
            [Blob("generated-images", Connection = "AzureWebJobsStorage")] CloudBlobContainer container,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Checking status for job {jobId}");

                // Ensure the container exists
                await container.CreateIfNotExistsAsync();

                // List blobs (images) with the prefix of jobId
                var blobList = container.ListBlobsSegmentedAsync(jobId, true, BlobListingDetails.Metadata, null, null, null, null);
                var results = await blobList;

                // Generate SAS URLs for each blob
                var sasUrls = results.Results.OfType<CloudBlockBlob>().Select(blob =>
                {
                    string sasToken = GetBlobSasUri(container, blob.Name);
                    return blob.Uri + sasToken;
                }).ToList();

                // Check if the job is completed based on the expected number of images
                string status = sasUrls.Count == 51 ? "Completed" : "InProgress"; // Assuming 51 images per job

                return new OkObjectResult(new { JobId = jobId, Status = status, ImageUrls = sasUrls });
            }
            catch (Exception ex)
            {
                log.LogError($"Error in JobStatusFunction: {ex.Message}");
                return new StatusCodeResult(500); // Internal Server Error
            }
        }

        private static string GetBlobSasUri(CloudBlobContainer container, string blobName, string policyName = null)
        {
            var blob = container.GetBlockBlobReference(blobName);

            var sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24), // 24-hour valid token
                Permissions = SharedAccessBlobPermissions.Read
            };

            string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints, policyName);
            return sasBlobToken;
        }
    }
}