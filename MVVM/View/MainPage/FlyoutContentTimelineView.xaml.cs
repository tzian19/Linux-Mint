using Linux_Mint.MVVM.ViewModel;

namespace Linux_Mint.MVVM.View.MainPage;

public partial class FlyoutContentTimelineView : ContentPage
{
    public FlyoutContentTimelineView()
    {
        InitializeComponent();

        // Get the logged-in user ID from your AppState or wherever you keep it
        var currentUserId = AppState.LoggedInUser?.UId ?? string.Empty;

        BindingContext = new FlyoutContentTimelineViewModel();
    }
}
