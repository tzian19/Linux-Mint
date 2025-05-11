using Linux_Mint.MVVM.View;
using Linux_Mint.MVVM.ViewModel;
namespace Linux_Mint.MVVM.view;

public partial class LoginView : ContentPage
{
    public LoginView()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();
        passEntry.Text = "1234";
        userEntry.Text = "tzian19";
    }

    private void showPassword_CheckedChanged( object sender , CheckedChangedEventArgs e )
    {
        if ( sender is CheckBox )
        {
            passEntry.IsPassword = !e.Value;
        }
    }
    private async void btnSignUp_Clicked( object sender , EventArgs e )
    {
        await MainThread.InvokeOnMainThreadAsync( async () =>
        {
            await Navigation.PushAsync( new SignUpView() );
        } );
    }

}