using System.Net.Http.Json;

using Linux_Mint.MVVM.Model;

public class PostService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://680f29be67c5abddd1940e6d.mockapi.io";

    public PostService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<UserPost>> GetPostsAsync()
    {
        try
        {
            var posts = await _httpClient.GetFromJsonAsync<List<UserPost>>(
                $"{BaseUrl}/UserPosts");

            return posts ?? new List<UserPost>();
        }
        catch ( Exception ex )
        {
            Console.WriteLine( $"Error fetching posts: {ex.Message}" );
            return new List<UserPost>();
        }
    }

    public async Task<List<UserProfile>> GetAllUsersAsync()
    {
        try
        {
            var users = await _httpClient.GetFromJsonAsync<List<UserProfile>>(
                $"{BaseUrl}/UserProfiles");

            return users ?? new List<UserProfile>();
        }
        catch ( Exception ex )
        {
            Console.WriteLine( $"Error fetching users: {ex.Message}" );
            return new List<UserProfile>();
        }
    }
}
