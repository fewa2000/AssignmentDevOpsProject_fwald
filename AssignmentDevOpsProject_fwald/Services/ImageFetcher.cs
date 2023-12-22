using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AssignmentDevOpsProject_fwald.Services
{
    public class ImageFetcher
    {
        private readonly HttpClient _httpClient;
        private readonly string _unsplashApiKey;

        public ImageFetcher(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _unsplashApiKey = apiKey;
        }

        public async Task<Stream> FetchImageAsync(string category = null)
        {
            try
            {
                string apiUrl = "https://api.unsplash.com/photos/random/?client_id=" + _unsplashApiKey + "&count=1";
                if (!string.IsNullOrEmpty(category))
                {
                    apiUrl += $"?query={Uri.EscapeDataString(category)}";
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Client-ID", _unsplashApiKey);

                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);

                string imageUrl = json["urls"]["regular"].ToString();

                if (!string.IsNullOrEmpty(imageUrl))
                {
                    var imageResponse = await _httpClient.GetAsync(imageUrl);
                    imageResponse.EnsureSuccessStatusCode();

                    // Return the image as a Stream
                    return await imageResponse.Content.ReadAsStreamAsync();
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

    }
}
