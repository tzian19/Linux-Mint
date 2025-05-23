using Linux_Mint.MVVM.ViewModel;

namespace Linux_Mint.MVVM.View.MainPage;

public partial class FlyoutContentNewPostView : ContentPage
{
    public FlyoutContentNewPostView()
    {
        InitializeComponent();
        BindingContext = new FlyoutContentNewPostViewModel();
    }

    private void OnRemoveImageClicked( object sender , EventArgs e )
    {
    }
}