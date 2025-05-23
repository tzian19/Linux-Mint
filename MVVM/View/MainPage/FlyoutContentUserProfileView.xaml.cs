using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.View.MainPage.ProfileContents;
using Linux_Mint.MVVM.ViewModel.MainPage;

using MauiView = Microsoft.Maui.Controls.View;

namespace Linux_Mint.MVVM.View.MainPage;

public partial class FlyoutContentUserProfileView : ContentPage
{
    public UserProfile currentUser;

    public FlyoutContentUserProfileView()
    {
        InitializeComponent();
        BindingContext = new FlyoutContentUserProfileViewModel( SetInitialContent );
    }

    private void SetInitialContent( MauiView view )
    {
        ContentContainer.Content = view;
    }

    private async Task ToolbarItem_ClickedAsync( object sender , EventArgs e )
    {
        // 1. Create a local helper to apply the colors
        Action applyMintStyle = () =>
        {
            if (Application.Current.MainPage is NavigationPage navPage)
            {
                navPage.BarBackgroundColor = Color.Parse("#E8FAEA");
                navPage.BarTextColor = Color.Parse("#217D41");
            }
            else if (Application.Current.MainPage is FlyoutPage flyout && flyout.Detail is NavigationPage detailNav)
            {
                detailNav.BarBackgroundColor = Color.Parse("#E8FAEA");
                detailNav.BarTextColor = Color.Parse("#217D41");
            }
        };

        // 2. Apply it immediately
        applyMintStyle();

        // 3. Push the page
        var pushTask = Navigation.PushAsync(new FlyoutContentNewPostView());

        // 4. "Keep on repeat" while the task is running (awaiting)
        while ( !pushTask.IsCompleted )
        {
            applyMintStyle();
            await Task.Delay( 100 ); // Small delay to prevent CPU melting
        }

        // Await the final result
        await pushTask;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if ( Application.Current.MainPage is FlyoutPage flyout && flyout.Detail is NavigationPage nav )
        {
            nav.BarBackgroundColor = Color.Parse( "#E8FAEA" );
            nav.BarTextColor = Color.Parse( "#217D41" );
        }
    }

    private void ToolbarItem_Clicked( object sender , EventArgs e )
    {
        _ = ToolbarItem_ClickedAsync( sender , e );
    }

    private void ContentContainer_Loaded( object sender , EventArgs e )
    {
        if ( ContentContainer.Content == null )
        {
            ContentContainer.Content = new UserPostedView();
        }
    }

}
