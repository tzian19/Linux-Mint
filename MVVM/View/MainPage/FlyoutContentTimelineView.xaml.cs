using Linux_Mint.MVVM.ViewModel;

namespace Linux_Mint.MVVM.View.MainPage;

public partial class FlyoutContentTimelineView : ContentPage
{
    public FlyoutContentTimelineView()
    {
        InitializeComponent();
        BindingContext = new FlyoutContentTimelineViewModel();
    }
}