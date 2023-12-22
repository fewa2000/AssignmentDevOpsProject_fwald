using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentDevOpsProject_fwald
{ 
public static class RequestImagesFunction
{
    [FunctionName("RequestImagesFunction")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        [Queue("imageprocessqueue", Connection = "AzureWebJobsStorage")] ICollector<string> msg,
        ILogger log)
    {
        try
        {
            log.LogInformation("HTTP trigger function to request image generation started.");

            var jobId = Guid.NewGuid().ToString();

            // Assuming there are 51 weather stations
            for (int i = 1; i <= 51; i++)
            {
                var jobDetails = new JobDetails
                {
                    JobId = jobId,
                    WeatherData = new WeatherStationData
                    {
                        WeatherStationId = i.ToString() // Assigning each station a unique ID
                    },
                    ImageData = new ImageData
                    {
                        ImageCategory = "nature" // Example category for image fetching
                    }
                };

                msg.Add(JsonConvert.SerializeObject(jobDetails));
            }

            return new OkObjectResult($"Job {jobId} created. Check status at https://assignmentdevopsproject_fwald.azurewebsites.net/api/jobstatus/{jobId}");
        }
        catch (Exception ex)
        {
            log.LogError($"Error in RequestImagesFunction: {ex.Message}");
            return new StatusCodeResult(500); // Internal Server Error
        }
    }
}
}

namespace AssignmentDevOpsProject_fwald
{
    public class JobDetails
    {
        public string JobId { get; set; }
        public WeatherStationData WeatherData { get; set; }
        public ImageData ImageData { get; set; }
    }
}

namespace AssignmentDevOpsProject_fwald
{
    public class WeatherStationData
    {
        public string WeatherStationId { get; set; }
        // Add other weather-related properties if needed
    }
}

namespace AssignmentDevOpsProject_fwald
{
    public class ImageData
    {
        public string ImageCategory { get; set; }
        // Add other image-related properties if needed
    }
}