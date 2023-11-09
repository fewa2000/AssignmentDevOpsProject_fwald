using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.IO;
using AssignmentDevOpsProject_fwald.Services;

public static class QueueTriggerFunctionStartingJob
{
    [FunctionName("ProcessStartJobQueue")]
    public static async Task ProcessStartJobQueue(
        [QueueTrigger("process-image-queue", Connection = "AzureWebJobsStorage")] string queueItem,
        [Queue("process-image-queue", Connection = "AzureWebJobsStorage")] ICollector<string> outputQueue,
        ILogger log)
    {
        log.LogInformation($"Processing queue item: {queueItem}");

        // Initialize your API services (make sure to manage dependencies appropriately)
        var buienradarAPI = new BuienraderAPI(new HttpClient());
        var unsplashAPI = new UnsplashAPI(new HttpClient(), "UnsplashApiKey");

        // Fetch weather data
        var weatherData = await buienradarAPI.GetWeatherDataAsync();

        // Assuming Unsplash API returns a list of image URLs
        var imageUrls = await unsplashAPI.GetImageUrlsAsync();

        foreach (var imageUrl in imageUrls)
        {

            string imageProcessingInfo = $"{{\"weatherData\": \"{weatherData}\", \"imageUrl\": \"{imageUrl}\"}}";

            outputQueue.Add(imageProcessingInfo);
        }

        
    }
}

