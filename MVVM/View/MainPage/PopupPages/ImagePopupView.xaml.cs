namespace Linux_Mint.MVVM.View.MainPage.PopupPages;

public partial class ImagePopupView : ContentPage
{
    public string ImageUrl { get; set; }

    public ImagePopupView( string imageUrl )
    {
        InitializeComponent();
        ImageUrl = imageUrl;
        BindingContext = this;
        // Start at a smaller scale
        MainLayout.Scale = 0.5;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Animate to full scale (zoom in)
        await MainLayout.ScaleTo( 1 , 600 , Easing.CubicOut );
    }

    private async void CloseButton_Clicked( object sender , EventArgs e )
    {
        await MainLayout.ScaleTo( 0.5 , 200 , Easing.CubicIn );
        await Navigation.PopModalAsync();
    }

}