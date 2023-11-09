using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AssignmentDevOpsProject_fwald.Services
{
    internal class UnsplashAPI
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public UnsplashAPI(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<byte[]> GetImageDataAsync()
        {
            try
            {
                string apiUrl = "https://api.unsplash.com/photos/random"; // Unsplash random photo endpoint
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Client-ID", _apiKey);

                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                var json = JArray.Parse(content); // Using JArray as the response starts with an array

                string imageUrl = json[0]["urls"]["regular"].ToString();

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    var imageResponse = await _httpClient.GetAsync(imageUrl);
                    imageResponse.EnsureSuccessStatusCode();

                    // Read the image data as a byte array
                    byte[] imageData = await imageResponse.Content.ReadAsByteArrayAsync();
                    return imageData; // Return the image data
                }
                else
                {
                    Console.WriteLine("No images found");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error fetching image: {e.Message}");
                return null;
            }
        }

        internal Task<IEnumerable<object>> GetImageUrlsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
