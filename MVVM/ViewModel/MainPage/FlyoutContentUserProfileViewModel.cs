using System.Windows.Input;

using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.View.MainPage.PopupPages;
using Linux_Mint.MVVM.View.MainPage.ProfileContents;

namespace Linux_Mint.MVVM.ViewModel.MainPage
{
    public class FlyoutContentUserProfileViewModel : ViewModelBase
    {
        public ICommand SwitchViewPage { get; }
        public ICommand ShowImageCommand { get; }


        private readonly Action<Microsoft.Maui.Controls.View> _updateViewCallback;

        public FlyoutContentUserProfileViewModel( Action<Microsoft.Maui.Controls.View> updateViewCallback )
        {
            ShowImageCommand = new Command<UserProfile>( ShowImagePopup );
            ;
            _updateViewCallback = updateViewCallback;

            SwitchViewPage = new Command<string>( async ( param ) => await SwitchViews( param ) );

            _updateViewCallback?.Invoke( new UserPostedView() );
        }
        private async void ShowImagePopup( UserProfile user )
        {
            if ( user == null || string.IsNullOrEmpty( user.UserAvatar ) )
                return;

            await Application.Current.MainPage.Navigation.PushModalAsync( new ImagePopupView( user.UserAvatar ) );
            ;
        }

        private async Task SwitchViews( string param )
        {
            Microsoft.Maui.Controls.View newView = param switch
            {
                "UserPostedView" => new UserPostedView(),
                "AboutContentView" => new AboutContentView(),
                _ => null
            };

            if ( newView != null )
            {
                _updateViewCallback?.Invoke( newView );
            }
        }
    }
}
