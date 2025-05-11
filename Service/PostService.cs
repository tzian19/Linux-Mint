using System.Net.Http.Json;

using Linux_Mint.MVVM.Model;

public class PostService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://64f7ddfd824680fd217fb676.mockapi.io/UserProfile/1/UserPosts";

    public PostService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<UserPost>> GetPostsAsync()
    {
        try
        {
            var posts = await _httpClient.GetFromJsonAsync<List<UserPost>>(BaseUrl);
            return posts ?? new List<UserPost>();
        }
        catch ( Exception ex )
        {
            Console.WriteLine( $"Error fetching posts: {ex.Message}" );
            return new List<UserPost>();
        }
    }
}
