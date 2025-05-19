using Linux_Mint.MVVM.view;
using Linux_Mint.MVVM.View.MainPage;
using System.Windows.Input;

namespace Linux_Mint.MVVM.ViewModel.MainPage
{
    public class MainFlyoutPageViewModel : ViewModelBase
    {
        public ICommand ClickProfileTab { get; }
        public ICommand ClickTimelineTab { get; }
        public ICommand ClickNewPostTab { get; }
        public ICommand ClickLogOut { get; }
        public ICommand FlyoutPageOnLoad { get; }
        public ICommand ClickMinimizeMenu { get; }
        public ICommand ClickHomeIcon { get; }

        public MainFlyoutPageViewModel()
        {
            LoggedInUser = AppState.LoggedInUser;
            FlyoutPageOnLoad = new Command(async () => await NavigateToPage(new FlyoutContentUserListView()));
            ClickProfileTab = new Command(async () => await NavigateToPage(new FlyoutContentUserProfileView()));
            ClickLogOut = new Command<object>(async (param) => await ParameterizedCommand(param));
            ClickTimelineTab = new Command(async () => await NavigateToPage(new FlyoutContentTimelineView()));
            ClickNewPostTab = new Command(async () => await NavigateToPage(new FlyoutContentNewPostView()));
            ClickMinimizeMenu = new Command(async () => await MinimizeMenu());
            ClickHomeIcon = new Command<object>(async (param) => await ParameterizedCommand(param));
        }

        private async Task ParameterizedCommand(object param)
        {
            //if ( param is string action2 && action2 == "Home" )
            //{
            //    NavigateToPage( new FlyoutContentTimelineView( _currentUser ) );
            //    MinimizeMenu();
            //    return;
            //}

            if (param is string action2 && action2 == "Home")
            {
                NavigateToPage(new FlyoutContentUserListView());
                MinimizeMenu();
                return;
            }

            bool response = await Application.Current.MainPage.DisplayAlert("Confirm", "Are you sure you wnat to logout?", "Yes", "No");
            if (response == false)
                return;

            if (param is string action && action == "Exit")
            {
                Application.Current.MainPage = new NavigationPage(new LoginView())
                {
                    BarBackgroundColor = Color.FromArgb("#46B47F"),
                    BarTextColor = Color.FromArgb("#94FFD4")
                };
                return;
            }
        }

        private async Task MinimizeMenu()
        {
            if (Application.Current.MainPage is FlyoutPage flyoutPage && DeviceInfo.Platform != DevicePlatform.WinUI)
            {
                flyoutPage.IsPresented = !flyoutPage.IsPresented; // Toggle flyout visibility
            }

            await Task.Delay(50);
        }

        private Color ColorPallete(string tag)
        {
            return Application.Current.Resources.TryGetValue(tag, out var color) && color is Color c
                     ? c
                     : Colors.Transparent;
        }

        private async Task NavigateToPage(ContentPage page)
        {
            if (Application.Current.MainPage is FlyoutPage flyout)
            {
                page.BackgroundColor = ColorPallete("Primary");
                flyout.Detail = new NavigationPage(page)
                {
                    BarBackgroundColor = ColorPallete("Primary"),
                    BarTextColor = ColorPallete("SecondaryDarkText")
                };
                await Task.Delay(2000);

                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    flyout.IsPresented = false;
                }
            }
        }
    }
}