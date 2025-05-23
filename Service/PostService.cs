using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using Linux_Mint.MVVM.Model;

public class PostService
{
    private readonly string _baseUserUrl = "https://680f29be67c5abddd1940e6d.mockapi.io/UserProfiles";

    protected internal readonly HttpClient _httpClient;
    protected internal string BaseUrl = "https://680f29be67c5abddd1940e6d.mockapi.io";

    public PostService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<UserPost>> GetPostsAsync()
    {
        try
        {
            var posts = await _httpClient.GetFromJsonAsync<List<UserPost>>($"{BaseUrl}/UserPosts");
            foreach ( var post in posts ?? new List<UserPost>() )
            {
                if ( post.LikedBy == null )
                    post.LikedBy = new List<string>();
            }

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

    public async Task<bool> UpdatePostAsync( UserPost post )
    {
        try
        {
            Console.WriteLine( $"⏳ Attempting to update post {post.PostId}" );

            if ( string.IsNullOrEmpty( post?.PostId ) )
            {
                Console.WriteLine( "❌ Update failed: Post ID is null or empty" );
                return false;
            }

            // CORRECTED: Proper endpoint URL construction
            var updateUrl = $"{BaseUrl}/UserPosts/{post.PostId}";
            Console.WriteLine( $"🔗 API Endpoint: {updateUrl}" );

            // Configure JSON serializer options
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var jsonContent = JsonSerializer.Serialize(post, options);
            Console.WriteLine( $"📦 Request Payload: {jsonContent}" );

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(updateUrl, content);

            Console.WriteLine( $"🔄 Response Status: {response.StatusCode}" );

            if ( !response.IsSuccessStatusCode )
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine( $"❌ Error Content: {errorContent}" );
                return false;
            }

            Console.WriteLine( "✅ Update successful!" );
            return true;
        }
        catch ( Exception ex )
        {
            Console.WriteLine( $"💥 Exception: {ex.Message}" );
            Console.WriteLine( $"🔍 Stack Trace: {ex.StackTrace}" );
            return false;
        }
    }

    public async Task<bool> DeletePostAsync( UserPost post )
    {
        try
        {
            Console.WriteLine( $"⏳ Attempting to delete post {post.PostId}" );

            if ( string.IsNullOrEmpty( post.PostId ) )
            {
                Console.WriteLine( "❌ Delete failed: Post ID is null or empty" );
                return false;
            }

            var deleteUrl = $"{BaseUrl}/UserPosts/{post.PostId}";
            Console.WriteLine( $"🔗 API Endpoint: {deleteUrl}" );

            var response = await _httpClient.DeleteAsync(deleteUrl);

            Console.WriteLine( $"🔄 Response Status: {response.StatusCode}" );

            if ( !response.IsSuccessStatusCode )
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine( $"❌ Error Content: {errorContent}" );
                return false;
            }

            Console.WriteLine( "✅ Delete successful!" );
            return true;
        }
        catch ( Exception ex )
        {
            Console.WriteLine( $"💥 Exception: {ex.Message}" );
            Console.WriteLine( $"🔍 Stack Trace: {ex.StackTrace}" );
            return false;
        }
    }
    public async Task<bool> CreatePostAsync( UserPost newPost )
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(newPost, options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{BaseUrl}/UserPosts", content);

            return response.IsSuccessStatusCode;
        }
        catch ( Exception ex )
        {
            Console.WriteLine( $"Error creating post: {ex.Message}" );
            return false;
        }

    }
    public async Task<bool> DeletePostAsync( string userId , string postId )
    {
        try
        {
            var url = $"{_baseUserUrl}/{userId}/UserPosts/{postId}";
            var response = await _httpClient.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }


}
