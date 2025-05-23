using System.Net.Http.Json;
using System.Text.Json; // <-- ADDED THIS

using Linux_Mint.MVVM.Model;

namespace Linux_Mint.Service
{
    public class PostService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions; // <-- ADDED THIS
        private const string BaseUrl = "https://680f29be67c5abddd1940e6d.mockapi.io";

        public PostService()
        {
            _httpClient = new HttpClient();

            // <-- ADDED THIS: Forces the app to keep your exact Capitalization (PascalCase)
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            };
        }

        public async Task<List<UserPost>> GetPostsAsync()
        {
            try
            {
                // 1. Fetch the posts (which might have stale UserProfile snapshots)
                var posts = await _httpClient.GetFromJsonAsync<List<UserPost>>($"{BaseUrl}/UserPosts", _jsonOptions);

                // 2. Fetch the FRESH user profiles
                var allUsers = await GetAllUsersAsync();

                foreach ( var post in posts ?? new List<UserPost>() )
                {
                    // Fix null LikedBy list
                    if ( post.LikedBy == null )
                    {
                        post.LikedBy = new List<string>();
                    }

                    // 3. THE FIX: Find the fresh user data that matches this post
                    var freshUser = allUsers.FirstOrDefault(u => u.UId == post.UserProfileId);

                    if ( freshUser != null )
                    {
                        // Overwrite the stale snapshot with the fresh data!
                        post.UserProfile = freshUser;

                    }
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
                // Passed _jsonOptions here
                var users = await _httpClient.GetFromJsonAsync<List<UserProfile>>($"{BaseUrl}/UserProfiles", _jsonOptions);
                return users ?? new List<UserProfile>();
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Error fetching users: {ex.Message}" );
                return new List<UserProfile>();
            }
        }

        public async Task<bool> CreatePostAsync( UserPost newPost )
        {
            try
            {
                // Passed _jsonOptions here
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/UserPosts", newPost, _jsonOptions);
                return response.IsSuccessStatusCode;
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Error creating post: {ex.Message}" );
                return false;
            }
        }

        public async Task<bool> UpdatePostAsync( UserPost post )
        {
            try
            {
                if ( string.IsNullOrEmpty( post?.PostId ) )
                {
                    Console.WriteLine( "❌ Update failed: Post ID is null or empty" );
                    return false;
                }

                // Passed _jsonOptions here
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/UserPosts/{post.PostId}", post, _jsonOptions);

                if ( !response.IsSuccessStatusCode )
                {
                    // This will print exactly WHY MockAPI rejected your Like!
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine( $"❌ API ERROR: {errorContent}" );
                }

                return response.IsSuccessStatusCode;
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"💥 Exception updating post: {ex.Message}" );
                return false;
            }
        }

        public async Task<bool> DeletePostAsync( string postId )
        {
            try
            {
                if ( string.IsNullOrEmpty( postId ) )
                {
                    Console.WriteLine( "❌ Delete failed: Post ID is null or empty" );
                    return false;
                }

                var response = await _httpClient.DeleteAsync($"{BaseUrl}/UserPosts/{postId}");
                return response.IsSuccessStatusCode;
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"💥 Exception deleting post: {ex.Message}" );
                return false;
            }
        }
    }
}