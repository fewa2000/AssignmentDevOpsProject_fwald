using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using AssignmentDevOpsProject_fwald.Services;

namespace AssignmentDevOpsProject_fwald
{
    public static class ImageGenerationFunction
    {
        [FunctionName("ImageGenerationFunction")]
        public static async Task Run(
            [QueueTrigger("imageprocessqueue", Connection = "AzureWebJobsStorage")] string queueItem,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Processing job: {queueItem}");

                var jobDetails = JsonConvert.DeserializeObject<JobDetails>(queueItem);

                // Retrieve the storage connection string from environment variables
                string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

                // Retrieve the Apikey from environment variables
                string unsplashApiKey = Environment.GetEnvironmentVariable("UnsplashApiKey");

                // Initialize the BlobStorage service
                var blobStorageService = new BlobStorage(storageConnectionString);

                // Initialize other services
                var weatherDataFetcher = new WeatherDataFetcher(new HttpClient());
                var imageFetcher = new ImageFetcher(new HttpClient(), unsplashApiKey); // Replace with your actual Unsplash API key

                // Fetch weather data
                var weatherData = await weatherDataFetcher.FetchWeatherDataAsync(jobDetails.WeatherData.WeatherStationId);
                if (weatherData == null)
                {
                    log.LogError("Failed to fetch weather data.");
                    return;
                }

                // Fetch base image as a Stream
                var baseImageStream = await imageFetcher.FetchImageAsync(jobDetails.ImageData.ImageCategory);
                if (baseImageStream == null)
                {
                    log.LogError("Failed to fetch base image.");
                    return;
                }

                // Process image with weather data
                var processedImageStream = ImageProcessor.AddWeatherDataToImage(baseImageStream, weatherData, (10, 10)); // Position can be adjusted

                // Blob container and file name
                string blobContainerName = "generated-images";
                string blobName = $"{jobDetails.JobId}-{jobDetails.WeatherData.WeatherStationId}.jpg";

                // Upload the processed image to Blob Storage and get SAS URL
                string sasUrl = await blobStorageService.UploadImageAsync(processedImageStream, blobContainerName, blobName);
                log.LogInformation($"Image uploaded for job {jobDetails.JobId}, SAS URL: {sasUrl}");
            }
            catch (Exception ex)
            {
                log.LogError($"Error in ImageGenerationFunction: {ex.Message}");
            }
        }
    }
}