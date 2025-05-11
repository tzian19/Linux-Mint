using Linux_Mint.MVVM.ViewModel.MainPage;

namespace Linux_Mint.MVVM.View.MainPage;

public partial class FlyoutContentUserListView : ContentPage
{
    public FlyoutContentUserListView()
    {
        InitializeComponent();
        BindingContext = new FlyoutContentUserListViewModel();
    }
}