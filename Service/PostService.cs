using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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

    public async Task<bool> UpdatePostAsync(UserPost post)
    {
        try
        {
            Console.WriteLine($"⏳ Attempting to update post {post.PostId}");

            if (string.IsNullOrEmpty(post?.PostId))
            {
                Console.WriteLine("❌ Update failed: Post ID is null or empty");
                return false;
            }

            var updateUrl = $"{BaseUrl} / {post.PostId}";
            Console.WriteLine($"🔗 API Endpoint: {updateUrl}");

            var jsonContent = JsonSerializer.Serialize(post);
            Console.WriteLine($"📦 Request Payload: {jsonContent}");

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(updateUrl, content);

            Console.WriteLine($"🔄 Response Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error Content: {errorContent}");
                return false;
            }

            Console.WriteLine("✅ Update successful!");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"💥 Exception: {ex.Message}");
            Console.WriteLine($"🔍 Stack Trace: {ex.StackTrace}");
            return false;
        }
    }
}
