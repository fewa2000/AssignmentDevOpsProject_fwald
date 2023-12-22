using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AssignmentDevOpsProject_fwald.Services
{
    public class WeatherDataFetcher
    {
        private readonly HttpClient _httpClient;

        public WeatherDataFetcher(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> FetchWeatherDataAsync(string weatherStationId)
        {
            try
            {
                string apiUrl = "https://data.buienradar.nl/2.0/feed/json";

                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                var json = JObject.Parse(content);
                // Assuming weatherStationId is used to select the specific station's data
                var stationData = json["actual"]["stationmeasurements"][weatherStationId];

                if (stationData != null)
                {
                    // Extract and return relevant weather data
                    // Modify this according to the structure of the JSON and your needs
                    string weatherInfo = stationData["weatherDescription"].ToString();
                    return weatherInfo;
                }
                else
                {
                    Console.WriteLine("No weather data found for the specified station.");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error fetching weather data: {e.Message}");
                return null;
            }
        }
    }
}
