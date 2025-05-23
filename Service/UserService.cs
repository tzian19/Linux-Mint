using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json; // <-- ADD THIS

using Linux_Mint.MVVM.Model;

namespace Linux_Mint.Service
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions; // <-- ADD THIS

        private const string BaseUrl = "https://680f29be67c5abddd1940e6d.mockapi.io/UserProfiles";

        public UserService()
        {
            _httpClient = new HttpClient();

            // This forces the app to keep your exact Capitalization (PascalCase)
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            };
        }

        public async Task<ObservableCollection<UserProfile>> GetUsersAsync()
        {
            try
            {
                // Apply options here
                var users = await _httpClient.GetFromJsonAsync<ObservableCollection<UserProfile>>(BaseUrl, _jsonOptions);
                return users ?? new ObservableCollection<UserProfile>();
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Error fetching users: {ex.Message}" );
                return new ObservableCollection<UserProfile>();
            }
        }

        public async Task<UserProfile> GetUserByIdAsync( string id )
        {
            try
            {
                // Apply options here
                return await _httpClient.GetFromJsonAsync<UserProfile>( $"{BaseUrl}/{id}" , _jsonOptions );
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Error fetching user by id: {ex.Message}" );
                return null;
            }
        }

        public async Task<bool> AddUserAsync( UserProfile user )
        {
            try
            {
                // Apply options here
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, user, _jsonOptions);
                return response.IsSuccessStatusCode;
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Error adding user: {ex.Message}" );
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync( UserProfile user )
        {
            try
            {
                if ( user == null || string.IsNullOrEmpty( user.UId ) )
                    return false;

                string updateUrl = $"{BaseUrl}/{user.UId}";

                // Apply options here
                var response = await _httpClient.PutAsJsonAsync(updateUrl, user, _jsonOptions);

                if ( response.IsSuccessStatusCode )
                {
                    return true;
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine( $"❌ API ERROR: {errorContent}" );
                    return false;
                }
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"💥 Exception in UpdateUserAsync: {ex.Message}" );
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync( string id )
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Error deleting user: {ex.Message}" );
                return false;
            }
        }
    }
}