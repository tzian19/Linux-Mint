using Linux_Mint.MVVM.ViewModel;

namespace Linux_Mint.MVVM.View
{
    public partial class SignUpView : ContentPage
    {
        public SignUpView()
        {
            InitializeComponent();
            BindingContext = new SignUpViewModel();
            BirthDatePicker.MaximumDate = DateTime.Today;
            BirthDatePicker.MaximumDate = DateTime.Today.AddYears(-18);
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            DateTime birthDate = e.NewDate;
            int age = DateTime.Now.Year - birthDate.Year;

            // Adjust age if birthday hasn't occurred yet this year
            if (birthDate > DateTime.Now.AddYears(-age))
                age--;

            entryAge.Text = age.ToString();

            // Disable Sign-Up button if age < 18
            btnSignUp.IsEnabled = age >= 18;
        }

        private void OnShowPasswordCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            entryPassword.IsPassword = !e.Value;
            entryReenterPassword.IsPassword = !e.Value;
        }

        private void entryReenterPassword_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Text" && e.PropertyName != null)
            {
                bool isMatch = entryPassword.Text == entryReenterPassword.Text;
                lblPassNotMacthedNotice1.IsVisible = !isMatch;
                lblPassNotMacthedNotice2.IsVisible = !isMatch;
            }
        }

        private async void btnLogin_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}