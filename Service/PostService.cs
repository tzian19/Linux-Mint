using Linux_Mint.MVVM.Model;
using System.Net.Http.Json;

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
            var users = await _httpClient.GetFromJsonAsync<List<UserProfile>>($"{BaseUrl}/UserProfiles");
            var posts = await _httpClient.GetFromJsonAsync<List<UserPost>>($"{BaseUrl}/UserPosts");

            if (users == null || posts == null)
                return new List<UserPost>();

            // Join post with user info
            foreach (var post in posts)
            {
                var user = users.FirstOrDefault(u => u.UId == post.UserId);
                if (user != null)
                {
                    post.FirstName = user.FirstName;
                    post.LastName = user.LastName;
                    post.UserAvatar = user.UserAvatar;
                    post.Email = user.Email;
                    post.Username = user.Username;
                    post.FullName = $"{user.FirstName} {user.LastName}";
                }
            }

            return posts;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching posts: {ex.Message}");
            return new List<UserPost>();
        }
    }
}
