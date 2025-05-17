using Linux_Mint.MVVM.Model;

namespace Linux_Mint.MVVM.View.MainPage;

public partial class FlyoutContentUserProfileView : ContentPage
{
    public UserProfile currentUser;
    public FlyoutContentUserProfileView()
    {
        InitializeComponent();
    }

    private Task ToolbarItem_ClickedAsync( object sender , EventArgs e )
    {
        return Navigation.PushAsync( new FlyoutContentNewPostView( currentUser ) );
    }

    // Update your XAML or event subscription to use the new async method with a fire-and-forget pattern:
    private void ToolbarItem_Clicked( object sender , EventArgs e )
    {
        _ = ToolbarItem_ClickedAsync( sender , e );
    }

    private void TimelineButton( object sender , EventArgs e )
    {
        TimelineContainer.IsVisible = true;
        AboutContainer.IsVisible = false;
    }

    private void AboutButton( object sender , EventArgs e )
    {
        TimelineContainer.IsVisible = false;
        AboutContainer.IsVisible = true;
    }
}