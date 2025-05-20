using Linux_Mint.MVVM.Model;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;

namespace Linux_Mint.MVVM.ViewModel.MainPage
{
    internal class FlyoutContentUserListViewModel : ViewModelBase
    {
        public ObservableCollection<UserProfile> Users { get; set; }

        public ICommand GetAllUsersCommand { get; }

        public FlyoutContentUserListViewModel()
        {
            Users = new ObservableCollection<UserProfile>();
            GetAllUsersCommand = new Command(async () => await LoadUsers());

            // Auto-load users on ViewModel construction
            _ = LoadUsers();
        }

        private async Task LoadUsers()
        {
            try
            {
                var response = await client.GetAsync($"{baseUrl}/UserProfiles"); // Adjust 'UserProfiles' if needed
                if (response.IsSuccessStatusCode)
                {
                    using var stream = await response.Content.ReadAsStreamAsync();
                    var users = await JsonSerializer.DeserializeAsync<List<UserProfile>>(stream, _serializerOptions);

                    if (users != null)
                    {
                        Users.Clear();
                        foreach (var user in users)
                        {
                            Users.Add(user);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Server error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}