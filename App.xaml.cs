using Linux_Mint.MVVM.view;
using Linux_Mint.Service;

namespace Linux_Mint
{
    public partial class App : Application
    {
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