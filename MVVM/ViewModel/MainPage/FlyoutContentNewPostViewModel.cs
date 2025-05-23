using System.Windows.Input;

using Linux_Mint.MVVM.Model;

namespace Linux_Mint.MVVM.ViewModel
{
    public class FlyoutContentNewPostViewModel : ViewModelBase
    {
        private readonly PostService _postService;

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
            // You can customize this to allow posting only when there's text or an image
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
                    var stream = await result.OpenReadAsync();
                    // Use local file path (on some platforms, FullPath may be null, so be cautious)
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
            ( ( Command ) PostCommand ).ChangeCanExecute();
        }

        private async Task OnPostAsync()
        {
            if ( !CanExecutePost() )
                return;

            var newPost = new UserPost
            {
                PostId = Guid.NewGuid().ToString(),
                PostText = PostTexts,
                PostImage = PostImage,
                PostCreated = DateTime.Now.ToString("o"), // ISO 8601 format
                UserProfileId = LoggedInUser?.UId,
                UserProfile = LoggedInUser,
                LikedBy = new System.Collections.Generic.List<string>(),
                CurrentUserId = LoggedInUser?.UId
            };

            try
            {
                // Post new post to backend (assuming POST endpoint exists)
                var json = System.Text.Json.JsonSerializer.Serialize(newPost);
                var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await _postService._httpClient.PostAsync($"{_postService.BaseUrl}/UserPosts", content);

                if ( response.IsSuccessStatusCode )
                {
                    // Add new post locally and clear input
                    Posts.Insert( 0 , newPost );

                    PostTexts = string.Empty;
                    PostImage = null;
                    ( ( Command ) PostCommand ).ChangeCanExecute();

                    // Optionally navigate back or inform the user
                    await Application.Current.MainPage.DisplayAlert( "Success" , "Post published!" , "OK" );
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert( "Error" , "Failed to publish post." , "OK" );
                }
            }
            catch ( Exception ex )
            {
                await Application.Current.MainPage.DisplayAlert( "Exception" , ex.Message , "OK" );
            }
        }
    }
}
