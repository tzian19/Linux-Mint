using System.Windows.Input;

using Linux_Mint.MVVM.Model;
using Linux_Mint.Service;

namespace Linux_Mint.MVVM.ViewModel
{
    public class AboutContentViewModel : ViewModelBase
    {
        private readonly UserService _userService;

        private UserProfile _loggedInUser;
        public UserProfile LoggedInUser
        {
            get => _loggedInUser;
            set => SetProperty( ref _loggedInUser , value );
        }

        // We use these to hold the original data in case they cancel
        private string _backupUsername;
        private DateTime _backupBirthDate;
        private string _backupEmail;

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                SetProperty( ref _isEditing , value );
                OnPropertyChanged( nameof( IsNotEditing ) );
            }
        }

        public bool IsNotEditing => !IsEditing;

        public ICommand ToggleEditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; } // NEW COMMAND

        public AboutContentViewModel()
        {
            _userService = new UserService();

            ToggleEditCommand = new Command( StartEditing );
            SaveCommand = new Command( async () => await SaveDetailsAsync() );
            CancelCommand = new Command( CancelEditing );
        }

        private void StartEditing()
        {
            // 1. Back up the data before the user starts typing
            if ( LoggedInUser != null )
            {
                _backupUsername = LoggedInUser.Username;
                _backupBirthDate = LoggedInUser.BirthDate;
                _backupEmail = LoggedInUser.Email;
            }

            // 2. Switch to edit mode
            IsEditing = true;
        }

        private void CancelEditing()
        {
            // 1. Restore the original data to undo any typing they did
            if ( LoggedInUser != null )
            {
                LoggedInUser.Username = _backupUsername;
                LoggedInUser.BirthDate = _backupBirthDate;
                LoggedInUser.Email = _backupEmail;

                // Tell the UI to refresh the labels
                OnPropertyChanged( nameof( LoggedInUser ) );
            }

            // 2. Switch back to view mode
            IsEditing = false;
        }

        private async Task SaveDetailsAsync()
        {
            IsEditing = false;

            bool success = await _userService.UpdateUserAsync(LoggedInUser);

            if ( success )
            {
                await Application.Current.MainPage.DisplayAlert( "Success" , "Profile updated!" , "OK" );
            }
            else
            {
                // If save fails, open edit mode back up
                IsEditing = true;
                await Application.Current.MainPage.DisplayAlert( "Error" , "Failed to save profile." , "OK" );
            }
        }
    }
}