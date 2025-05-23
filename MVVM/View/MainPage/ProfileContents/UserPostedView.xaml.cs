using Linux_Mint.MVVM.ViewModel.MainPage.ProfileContents;

namespace Linux_Mint.MVVM.View.MainPage.ProfileContents;

public partial class UserPostedView : ContentView
{
    public UserPostedView()
    {
        InitializeComponent();
        BindingContext = new UserPostedViewModel();
    }
}
