using System.Collections.ObjectModel;
using System.Net.Http.Json;

using Linux_Mint.MVVM.Model;

namespace Linux_Mint.Service
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://64f7ddfd824680fd217fb676.mockapi.io/UserProfiles";

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
                Console.WriteLine( $"Error fetching users: {ex.Message}" );
                return new ObservableCollection<UserProfile>();
            }
        }

        public async Task<UserProfile> GetUserByIdAsync( string id )
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserProfile>( $"{BaseUrl}/{id}" );
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
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, user);
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
