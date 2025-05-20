using Linux_Mint.MVVM.ViewModel.MainPage.ProfileContents;

namespace Linux_Mint.MVVM.View.MainPage.ProfileContents;

public partial class AboutContentView : ContentView
{
    public AboutContentView()
    {
        InitializeComponent();
        BindingContext = new AboutContentViewModel();
    }
}