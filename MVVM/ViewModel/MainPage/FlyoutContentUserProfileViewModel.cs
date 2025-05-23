using System.Windows.Input;

using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.View.MainPage.PopupPages;
using Linux_Mint.MVVM.View.MainPage.ProfileContents;
using Linux_Mint.Service; // Needed for UserService and ImgurService

namespace Linux_Mint.MVVM.ViewModel.MainPage
{
    public class FlyoutContentUserProfileViewModel : ViewModelBase
    {
        private readonly UserService _userService;
        private readonly ImgurService _imgurService; // NEW: Added ImgurService
        private readonly Action<Microsoft.Maui.Controls.View> _updateViewCallback;

        // --- EXISTING COMMANDS ---
        public ICommand SwitchViewPage { get; }
        public ICommand ShowImageCommand { get; }

        // --- NEW EDIT STATE BOOLEANS ---
        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                SetProperty( ref _isEditing , value );
                OnPropertyChanged( nameof( IsNotEditing ) ); // Keeps the View mode in sync
            }
        }
        public bool IsNotEditing => !IsEditing;

        // --- BACKUP VARIABLES FOR CANCEL ---
        private string _backupUsername;
        private DateTime _backupBirthDate;
        private string _backupEmail;

        // --- NEW EDIT COMMANDS ---
        public ICommand ToggleEditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // NEW: Command for changing the profile photo
        public ICommand ChangePhotoCommand { get; }

        public FlyoutContentUserProfileViewModel( Action<Microsoft.Maui.Controls.View> updateViewCallback )
        {
            _userService = new UserService();
            _imgurService = new ImgurService(); // NEW: Initialize Imgur
            _updateViewCallback = updateViewCallback;

            // Existing Commands
            ShowImageCommand = new Command<UserProfile>( ShowImagePopup );
            SwitchViewPage = new Command<string>( async ( param ) => await SwitchViews( param ) );

            // New Edit Commands
            ToggleEditCommand = new Command( StartEditing );
            SaveCommand = new Command( async () => await SaveDetailsAsync() );
            CancelCommand = new Command( CancelEditing );

            // 🟢 NEW: Initialize Change Photo Command
            ChangePhotoCommand = new Command( async () => await ChangePhotoAsync() );

            // Load the default view
            _updateViewCallback?.Invoke( new UserPostedView() );
        }

        private async void ShowImagePopup( UserProfile user )
        {
            if ( user == null || string.IsNullOrEmpty( user.UserAvatar ) )
                return;

            await Application.Current.MainPage.Navigation.PushModalAsync( new ImagePopupView( user.UserAvatar ) );
        }

        private async Task SwitchViews( string param )
        {
            // Reset edit mode if they switch tabs back and forth
            IsEditing = false;

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

        // --- NEW EDITING LOGIC ---
        private void StartEditing()
        {
            // 1. Back up data (using the inherited LoggedInUser from ViewModelBase)
            if ( LoggedInUser != null )
            {
                _backupUsername = LoggedInUser.Username;
                _backupBirthDate = LoggedInUser.BirthDate;
                _backupEmail = LoggedInUser.Email;
            }
            IsEditing = true;
        }

        private void CancelEditing()
        {
            // 1. Restore original data
            if ( LoggedInUser != null )
            {
                LoggedInUser.Username = _backupUsername;
                LoggedInUser.BirthDate = _backupBirthDate;
                LoggedInUser.Email = _backupEmail;

                // Tell the UI to refresh the labels
                OnPropertyChanged( nameof( LoggedInUser ) );
            }
            IsEditing = false;
        }

        private async Task SaveDetailsAsync()
        {
            try
            {
                bool success = await _userService.UpdateUserAsync(LoggedInUser);

                if ( success )
                {
                    // 1. ADDED THIS: Forces the Profile View labels (Name, Username) to update instantly!
                    OnPropertyChanged( nameof( LoggedInUser ) );

                    // 2. Broadcast the updated user to the rest of the app (like the Flyout menu)
                    MessagingCenter.Send( this , "ProfileUpdated" , LoggedInUser );

                    IsEditing = false;
                    await Application.Current.MainPage.DisplayAlert( "Success" , "Profile updated!" , "OK" );
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert( "Error" , "Failed to save profile. Check your API connection." , "OK" );
                }
            }
            catch ( Exception ex )
            {
                await Application.Current.MainPage.DisplayAlert( "Exception" , $"An error occurred: {ex.Message}" , "OK" );
            }
        }

        // --- 🟢 NEW: PHOTO UPLOAD WITH CROPPER RE-ENABLED ---
        private async Task ChangePhotoAsync()
        {
            try
            {
                // 1. Pick the photo using the phone's native gallery
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Pick a new profile photo"
                });

                if ( result != null )
                {
                    // 2. Send it to the Cropper to force a perfect 1:1 Square
                    string cropResultFilePath = await Plugin.Maui.ImageCropper.Cropper.Current.Crop(new Plugin.Maui.ImageCropper.CropSettings
                    {
                        AspectRatioX = 1,
                        AspectRatioY = 1,
                        CropShape = Plugin.Maui.ImageCropper.CropSettings.CropShapeType.Rectangle
                    }, result.FullPath);

                    // 3. Check if they actually cropped it (didn't hit cancel)
                    if ( !string.IsNullOrEmpty( cropResultFilePath ) )
                    {
                        // Read the newly cropped, smaller image
                        byte[] imageBytes = System.IO.File.ReadAllBytes(cropResultFilePath);

                        // 4. Upload the square image directly to Imgur
                        string newAvatarUrl = await _imgurService.UploadImageAsync(imageBytes);

                        if ( !string.IsNullOrEmpty( newAvatarUrl ) )
                        {
                            // 5. Update the user model locally
                            LoggedInUser.UserAvatar = newAvatarUrl;

                            // Forces the Profile View Image to refresh instantly!
                            OnPropertyChanged( nameof( LoggedInUser ) );

                            // 6. Save the new URL to your database
                            bool success = await _userService.UpdateUserAsync(LoggedInUser);

                            if ( success )
                            {
                                // 📢 Broadcast the new photo to the Main Flyout Menu!
                                MessagingCenter.Send( this , "ProfileUpdated" , LoggedInUser );

                                await Application.Current.MainPage.DisplayAlert( "Success" , "Profile photo cropped and updated!" , "OK" );
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert( "Error" , "Photo uploaded, but failed to save to database." , "OK" );
                            }
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert( "Error" , "Failed to upload photo to Imgur." , "OK" );
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                await Application.Current.MainPage.DisplayAlert( "Error" , $"Photo process failed: {ex.Message}" , "OK" );
            }
        }

    }
}