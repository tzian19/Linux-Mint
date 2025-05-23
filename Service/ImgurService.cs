using System.Net.Http.Headers;
using System.Text.Json;

namespace Linux_Mint.Service
{
    public class ImgurService
    {
        private readonly HttpClient _httpClient;

        // Your specific Client ID. The Secret Key is intentionally left out!
        private const string ClientId = "de0bae3668b9b87";

        public ImgurService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Uploads an image byte array to Imgur and returns the direct web URL.
        /// </summary>
        public async Task<string> UploadImageAsync( byte [ ] imageBytes )
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.imgur.com/3/image");

                // This header authorizes the anonymous upload
                request.Headers.Authorization = new AuthenticationHeaderValue( "Client-ID" , ClientId );

                var content = new MultipartFormDataContent();
                content.Add( new ByteArrayContent( imageBytes ) , "image" );

                request.Content = content;
                var response = await _httpClient.SendAsync(request);

                if ( response.IsSuccessStatusCode )
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);

                    // Extracts the direct link (e.g., https://i.imgur.com/xyz.jpg)
                    return doc.RootElement.GetProperty( "data" ).GetProperty( "link" ).GetString();
                }

                Console.WriteLine( $"Imgur API Error: {response.StatusCode}" );
                return null;
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Imgur Upload Exception: {ex.Message}" );
                return null;
            }
        }
    }
}