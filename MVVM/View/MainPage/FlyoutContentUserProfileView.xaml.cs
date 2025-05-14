using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.ViewModel.MainPage;

namespace Linux_Mint.MVVM.View.MainPage;

public partial class FlyoutContentUserProfileView : ContentPage
{
	public FlyoutContentUserProfileView()
	{
		InitializeComponent();
	}

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {

    }

    private void TimelineButton(object sender, EventArgs e)
    {
        TimelineContainer.IsVisible = true;
        AboutContainer.IsVisible = false;
    }

    private void AboutButton(object sender, EventArgs e)
    {
        TimelineContainer.IsVisible = false;
        AboutContainer.IsVisible = true;
    }
}