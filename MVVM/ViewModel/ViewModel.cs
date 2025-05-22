using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;

using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.View.MainPage.PopupPages;
using Linux_Mint.Service;

namespace Linux_Mint.MVVM.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public ObservableCollection<UserPost> Posts { get; set; } = new();
        protected readonly HttpClient client;
        protected readonly JsonSerializerOptions _serializerOptions;
        protected readonly string baseUrl = "https://680f29be67c5abddd1940e6d.mockapi.io";

        private UserProfile _loggedInUser;

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty( ref _isRefreshing , value );
        }
        public UserProfile LoggedInUser
        {
            get => _loggedInUser;
            set => SetProperty( ref _loggedInUser , value );
        }

        public ViewModelBase()
        {
            DependencyService.Get<IKeepScreenOnService>()?.KeepScreenOn();
            LoggedInUser = AppState.LoggedInUser;
            OnSwiped = new Command<object>( async ( param ) => await SwipeTask( param ) );
            client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions { WriteIndented = true };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged( [CallerMemberName] string propertyName = null )
        {
            PropertyChanged?.Invoke( this , new PropertyChangedEventArgs( propertyName ) );
        }

        protected bool SetProperty<T>( ref T backingStore , T value , [CallerMemberName] string propertyName = "" )
        {
            if ( EqualityComparer<T>.Default.Equals( backingStore , value ) )
                return false;

            backingStore = value;
            OnPropertyChanged( propertyName );
            return true;
        }

        public ICommand OnSwiped { get; }

        private async Task SwipeTask( object param )
        {
            if ( param is string dir && dir == "Right" && Application.Current.MainPage is FlyoutPage flyoutPage )
            {
                flyoutPage.IsPresented = true;
            }
        }

        private Color ColorPallete( string tag )
        {
            return Application.Current.Resources.TryGetValue( tag , out var color ) && color is Color c
                     ? c
                     : Colors.Transparent;
        }

        internal async Task NavigateToPage( ContentPage page )
        {
            if ( Application.Current.MainPage is FlyoutPage flyout )
            {
                page.BackgroundColor = ColorPallete( "Primary" );
                flyout.Detail = new NavigationPage( page )
                {
                    BarBackgroundColor = ColorPallete( "Primary" ) ,
                    BarTextColor = ColorPallete( "SecondaryDarkText" )
                };
                await Task.Delay( 2000 );

                if ( DeviceInfo.Platform == DevicePlatform.Android )
                {
                    flyout.IsPresented = false;
                }
            }
        }

        protected internal async void ShowImagePopup( UserPost post )
        {
            if ( post == null || string.IsNullOrEmpty( post.PostImage ) )
                return;

            await Application.Current.MainPage.Navigation.PushModalAsync( new ImagePopupView( post.PostImage ) );
        }
    }
}
