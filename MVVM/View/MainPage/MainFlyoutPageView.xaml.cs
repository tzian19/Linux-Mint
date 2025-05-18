using Linux_Mint.MVVM.ViewModel.MainPage;

namespace Linux_Mint.MVVM.View.MainPage;

public partial class MainFlyoutPageView : FlyoutPage
{
    public MainFlyoutPageView()
    {

        InitializeComponent();
        BindingContext = new MainFlyoutPageViewModel();

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