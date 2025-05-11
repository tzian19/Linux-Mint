using System.Collections.ObjectModel;
using System.Net.Http.Json;

using Linux_Mint.MVVM.Model;

namespace Linux_Mint.Service
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://64f7ddfd824680fd217fb676.mockapi.io/api/v1/users";

        public UserService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<ObservableCollection<UserProfile>> GetUsersAsync()
        {
            try
            {
                var users = await _httpClient.GetFromJsonAsync<ObservableCollection<UserProfile>>(BaseUrl);
                return users ?? new ObservableCollection<UserProfile>();
            }
            catch ( Exception ex )
            {
                // Handle errors (maybe log or show UI alert)
                Console.WriteLine( $"Error fetching users: {ex.Message}" );
                return new ObservableCollection<UserProfile>();
            }
        }

        public async Task<bool> AddUserAsync( UserProfile newUser )
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, newUser);
                return response.IsSuccessStatusCode;
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Error adding user: {ex.Message}" );
                return false;
            }
        }

        public async Task<UserProfile> GetUserByIdAsync( string id )
        {
            try
            {
                var user = await _httpClient.GetFromJsonAsync<UserProfile>($"{BaseUrl}/{id}");
                return user;
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Error fetching user by id: {ex.Message}" );
                return null;
            }
        }

        public async Task<bool> UpdateUserAsync( UserProfile user )
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{user.UId}", user);
                return response.IsSuccessStatusCode;
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Error updating user: {ex.Message}" );
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
