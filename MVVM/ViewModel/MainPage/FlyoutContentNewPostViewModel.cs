using System.Windows.Input;

using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.View.MainPage;
using Linux_Mint.Service; // Ensure this is pointing to your services

namespace Linux_Mint.MVVM.ViewModel
{
    public class FlyoutContentNewPostViewModel : ViewModelBase
    {
        private readonly PostService _postService;
        private readonly ImgurService _imgurService; // 1. Add Imgur Service
        private byte[] _selectedImageBytes; // 2. Variable to hold the raw image data

        private string _postTexts;
        public string PostTexts
        {
            get => _postTexts;
            set => SetProperty( ref _postTexts , value );
        }

        private string _postImage;
        public string PostImage
        {
            get => _postImage;
            set => SetProperty( ref _postImage , value );
        }

        public ICommand AddPhotoCommand { get; }
        public ICommand RemoveImageCommand { get; }
        public ICommand PostCommand { get; }

        public FlyoutContentNewPostViewModel()
        {
            _postService = new PostService();
            _imgurService = new ImgurService();

            // Commands
            AddPhotoCommand = new Command( OnAddPhoto );
            RemoveImageCommand = new Command( OnRemoveImage );
            PostCommand = new Command( async () => await OnPostAsync() , CanExecutePost );

            // Initialize properties
            PostTexts = string.Empty;
            PostImage = null;
        }

        private bool CanExecutePost()
        {
            return !string.IsNullOrWhiteSpace( PostTexts ) || !string.IsNullOrEmpty( PostImage );
        }

        private async void OnAddPhoto()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Pick a photo"
                });

                if ( result != null )
                {
                    // 3. Convert the photo stream to a byte array so Imgur can read it later
                    using var stream = await result.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync( memoryStream );
                    _selectedImageBytes = memoryStream.ToArray();

                    // Keep this so your UI can still show a local preview of the image
                    PostImage = result.FullPath ?? "";

                    ( ( Command ) PostCommand ).ChangeCanExecute();
                }
            }
            catch ( Exception ex )
            {
                await Application.Current.MainPage.DisplayAlert( "Error" , $"Photo picking failed: {ex.Message}" , "OK" );
            }
        }

        private void OnRemoveImage()
        {
            PostImage = null;
            _selectedImageBytes = null; // Clear the bytes too
            ( ( Command ) PostCommand ).ChangeCanExecute();
        }

        private async Task OnPostAsync()
        {
            if ( !CanExecutePost() )
                return;

            string finalImgurUrl = "";

            // 4. Upload to Imgur ONLY when the user clicks Post
            if ( _selectedImageBytes != null )
            {
                // Optional: You might want to add a loading indicator/spinner here
                finalImgurUrl = await _imgurService.UploadImageAsync( _selectedImageBytes );

                if ( string.IsNullOrEmpty( finalImgurUrl ) )
                {
                    await Application.Current.MainPage.DisplayAlert( "Error" , "Failed to upload image to Imgur. Check your internet connection." , "OK" );
                    return; // Stop the post process if the image upload fails
                }
            }

            var newPost = new UserPost
            {
                PostId = Guid.NewGuid().ToString(),
                PostText = PostTexts,
                PostImage = finalImgurUrl, // 5. Save the Imgur URL, NOT the local file path!
                PostCreated = DateTime.Now.ToString("o"),
                UserProfileId = LoggedInUser?.UId,
                UserProfile = LoggedInUser,
                LikedBy = new System.Collections.Generic.List<string>(),
                CurrentUserId = LoggedInUser?.UId
            };

            try
            {
                // 6. Use the clean method from your PostService
                bool isSuccess = await _postService.CreatePostAsync(newPost);

                if ( isSuccess )
                {
                    // Add new post locally and clear input
                    Posts.Insert( 0 , newPost );

                    PostTexts = string.Empty;
                    PostImage = null;
                    _selectedImageBytes = null;
                    ( ( Command ) PostCommand ).ChangeCanExecute();

                    await Application.Current.MainPage.DisplayAlert( "Success" , "Post published!" , "OK" );
                    await NavigateToPage( new FlyoutContentTimelineView() );
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert( "Error" , "Failed to publish post to database." , "OK" );
                }
            }
            catch ( Exception ex )
            {
                await Application.Current.MainPage.DisplayAlert( "Exception" , ex.Message , "OK" );
            }
        }
    }
}