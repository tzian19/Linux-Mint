using System.Windows.Input;
using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.View.MainPage;
using Linux_Mint.MVVM.View.MainPage.ProfileContents;

namespace Linux_Mint.MVVM.ViewModel.MainPage
{
    public class FlyoutContentNewPostViewModel : ViewModelBase
    {

        private UserProfile _userProfile;
        private readonly PostService _postService;

        private string _postText;
        public string PostText
        {
            get => _postText;
            set => SetProperty(ref _postText, value);
        }

        private string _postImage;
        public string PostImage
        {
            get => _postImage;
            set => SetProperty(ref _postImage, value);
        }

        public ICommand AddPostCommand { get; }
        public ICommand AddPhotoCommand { get; }

        public FlyoutContentNewPostViewModel()
        {
            _postService = new PostService();

            AddPostCommand = new Command(async () => await AddPost());
            AddPhotoCommand = new Command(async () => await AddPhoto());
        }

        private async Task AddPost()
        {
            if (string.IsNullOrWhiteSpace(PostText))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Post text cannot be empty", "OK");
                return;
            }

            try
            {
                var newPost = new UserPost
                {
                    PostId = Guid.NewGuid().ToString(),
                    PostText = PostText,
                    PostImage = PostImage,
                    UserProfileId = AppState.LoggedInUser.UId,
                    PostCreated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                    LikeCount = 0,

                };

                var success = await _postService.CreatePostAsync(newPost);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Post created successfully!", "OK");
                    // Clear fields after successful post
                    PostText = string.Empty;
                    PostImage = null;


                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to create post", "OK");
                }
                await NavigateToPage(new FlyoutContentTimelineView());
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async Task AddPhoto()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync();
                if (result != null)
                {
                    var stream = await result.OpenReadAsync();
                    // You might want to upload this to a server or just use local path
                    PostImage = result.FullPath;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Could not get photo: {ex.Message}", "OK");
            }
        }
    }
}