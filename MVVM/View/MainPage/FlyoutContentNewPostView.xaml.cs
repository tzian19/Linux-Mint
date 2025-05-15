using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.ViewModel;
using Linux_Mint.MVVM.ViewModel.MainPage;


namespace Linux_Mint.MVVM.View.MainPage;

public partial class FlyoutContentNewPostView : ContentPage
{
    public FlyoutContentNewPostView()
    {
        BindingContext = new FlyoutContentNewPostViewModel();
        InitializeComponent();
    }

    private void OnRemoveImageClicked(object sender, EventArgs e)
    {
        

    }
}