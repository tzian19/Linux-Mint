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

        this.Flyout.IconImageSource = new FontImageSource
        {
            // Use "Fontello" if you have a specific icon, otherwise OpenSans works for the text burger
            FontFamily = "OpenSansSemibold" ,
            Glyph = "?" ,

            // Your exact Green Hex Code
            Color = Color.FromArgb( "#46B47F" ) ,

            Size = 30
        };
    }

    public async void OnLoaded( object sender , EventArgs e )
    {
        if ( BindingContext is MainFlyoutPageViewModel vm )
            vm.FlyoutPageOnLoad.Execute( null );


    }


}