using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.ViewModel.MainPage;

namespace Linux_Mint.MVVM.View.MainPage;

public partial class MainFlyoutPageView : FlyoutPage
{
    public MainFlyoutPageView( UserProfile user )
    {

        InitializeComponent();
        BindingContext = new MainFlyoutPageViewModel( user );

        if ( DeviceInfo.Platform == DevicePlatform.WinUI )
        {
            btnHideMenuIcon.IsVisible = false;
        }
    }
    public async void OnLoaded( object sender , EventArgs e )
    {
        if ( BindingContext is MainFlyoutPageViewModel vm )
            vm.FlyoutPageOnLoad.Execute( null );
    }
}