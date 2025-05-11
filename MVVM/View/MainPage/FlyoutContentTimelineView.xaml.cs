using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.ViewModel.MainPage;

namespace Linux_Mint.MVVM.View.MainPage;

public partial class FlyoutContentTimelineView : ContentPage
{
    public FlyoutContentTimelineView( UserProfile currentUser )
    {
        InitializeComponent();
        BindingContext = new FlyoutContentTimelineViewModel( currentUser );
    }
}