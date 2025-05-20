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

    private Task ToolbarItem_ClickedAsync( object sender , EventArgs e )
    {
        return Navigation.PushAsync( new FlyoutContentNewPostView() );
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


    // Optional: Uncomment if you decide to use MessagingCenter later
    //protected override void OnAppearing()
    //{
    //    base.OnAppearing();
    //    MessagingCenter.Subscribe<FlyoutContentUserProfileViewModel, MauiView>(
    //        this,
    //        "SwitchProfileContent",
    //        (sender, view) =>
    //        {
    //            ContentContainer.Content = view;
    //        });
    //}

    //protected override void OnDisappearing()
    //{
    //    base.OnDisappearing();
    //    MessagingCenter.Unsubscribe<FlyoutContentUserProfileViewModel, MauiView>(this, "SwitchProfileContent");
    //}
}
