using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.view;
using Linux_Mint.MVVM.View.MainPage;
using Linux_Mint.Service;

namespace Linux_Mint
{
    public partial class App : Application
    {
        UserProfile user = new UserProfile();
        public App()
        {
            InitializeComponent();
            DependencyService.Get<IKeepScreenOnService>()?.KeepScreenOn();
            MainPage = new NavigationPage( new LoginView() )
            {
                BarBackgroundColor = Color.FromArgb( "#46B47F" ) ,
                BarTextColor = Color.FromArgb( "#94FFD4" )
            };




        }


        protected override Window CreateWindow( IActivationState? activationState )
        {
            return new Window( MainPage );
        }
    }
}