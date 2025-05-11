using System.Text;
using System.Text.Json;
using System.Windows.Input;

using Linux_Mint.MVVM.Model;

namespace Linux_Mint.MVVM.ViewModel
{
    public class SignUpViewModel : ViewModelBase
    {
        private string _firstName;
        private string _lastName;
        private DateTime _birthDate = DateTime.Today;
        private int _age;
        private string _username;
        private string _password;
        private string _email;



        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(); } }
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(); } }
        public DateTime BirthDate { get => _birthDate; set { _birthDate = value; CalculateAge(); OnPropertyChanged(); } }
        public int Age { get => _age; private set { _age = value; OnPropertyChanged(); } }
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }

        public ICommand SignUpCommand { get; }

        public SignUpViewModel()
        {
            SignUpCommand = new Command( async () => await SignUp() );
        }

        private void CalculateAge()
        {
            int age = DateTime.Today.Year - BirthDate.Year;
            if ( BirthDate > DateTime.Today.AddYears( -age ) )
                age--;
            Age = age;
        }
        private async Task SignUp()
        {

            if ( string.IsNullOrWhiteSpace( FirstName ) || string.IsNullOrWhiteSpace( LastName ) ||
                string.IsNullOrWhiteSpace( Username ) || string.IsNullOrWhiteSpace( Password ) ||
                string.IsNullOrWhiteSpace( Email ) || !Email.Contains( "@" ) )
            {
                await Application.Current.MainPage.DisplayAlert( "Error" , "Please fill all fields correctly." , "OK" );
                return;
            }

            try
            {



                var response = await client.GetAsync($"{baseUrl}/UserProfiles");
                if ( response.IsSuccessStatusCode )
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    var users = await JsonSerializer.DeserializeAsync<List<UserProfile>>(stream, _serializerOptions);

                    if ( users.Any( u => u.Username == Username ) )
                    {
                        await Application.Current.MainPage.DisplayAlert( "Error" , "Username already exists." , "OK" );
                        return;
                    }

                    var newUser = new UserProfile
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        BirthDate = BirthDate,
                        Age = Age,
                        Username = Username,
                        Password = Password,
                        Email = Email,
                        UId = Guid.NewGuid().ToString()
                    };

                    var json = JsonSerializer.Serialize(newUser, _serializerOptions);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var postResponse = await client.PostAsync($"{baseUrl}/UserProfiles", content);
                    if ( postResponse.IsSuccessStatusCode )
                    {
                        await Application.Current.MainPage.DisplayAlert( "Success" , "Sign-up successful!" , "OK" );
                        await Application.Current.MainPage.Navigation.PopAsync();
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert( "Error" , "Failed to sign up." , "OK" );
                    }
                }
            }
            catch ( Exception ex )
            {
                await Application.Current.MainPage.DisplayAlert( "Error" , "Server error" , "OK" );
            }
        }
    }
}
