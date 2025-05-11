using System.Text.Json;
using System.Windows.Input;

using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.View.MainPage;
using Linux_Mint.Service;

namespace Linux_Mint.MVVM.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private string _inputUser;
        private string _inputPass;


        public string InputUser { get => _inputUser; set { _inputUser = value; OnPropertyChanged(); } }
        public string InputPass { get => _inputPass; set { _inputPass = value; OnPropertyChanged(); } }


        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command( async () => await Login() );

        }

        private async Task Login()
        {
            try
            {
                var response = await client.GetAsync($"{baseUrl}/UserProfiles");

                if ( response.IsSuccessStatusCode )
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    var users = await JsonSerializer.DeserializeAsync<List<UserProfile>>(stream, _serializerOptions);
                    var user = users?.FirstOrDefault(u => u.Username == InputUser && u.Password == InputPass);

                    if ( user != null )
                    {
                        LoggedInUser = user;

                        await Application.Current.MainPage.DisplayAlert( "Login Successfully" , $"Welcome! {user.FirstName} {user.LastName}" , "OK" );
                        await Task.Delay( 100 );

                        DependencyService.Get<IKeepScreenOnService>()?.KeepScreenOn();

                        Application.Current.MainPage = new MainFlyoutPageView( user );
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert( "Error" , "Invalid credentials" , "OK" );
                        //await Application.Current.MainPage.DisplayAlert("Error", "Invalid credentials", "OK");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert( "Error" , "Can't connect to the server" , "OK" );
                }
            }
            catch ( Exception ex )
            {
                await Application.Current.MainPage.DisplayAlert( "Error" , "Server error" , "OK" );
            }

        }
    }
}