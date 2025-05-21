using System.Windows.Input;
using Linux_Mint.MVVM.Model;
using Linux_Mint.Service;

namespace Linux_Mint.MVVM.ViewModel
{
    public class EditPostViewModel : ViewModelBase
    {
        private readonly PostService _postService;
        private UserPost _originalPost;
        private string _postText;

        public string PostText
        {
            get => _postText;
            set
            {
                _postText = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public bool IsBusy { get; private set; }

        public EditPostViewModel(UserPost post)
        {
            _postService = new PostService(); // Initialize your service
            _originalPost = post;
            PostText = post.PostText;

            SaveCommand = new Command(async () => await Save());
            CancelCommand = new Command(async () => await Cancel());
        }

        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(PostText))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Post cannot be empty", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                // Update local post
                _originalPost.PostText = PostText;
                _originalPost.PostCreated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

                // Update post in MockAPI
                bool success = await _postService.UpdatePostAsync(_originalPost);

                if (success)
                {
                    // Show success message before closing
                    await Application.Current.MainPage.DisplayAlert(
                        "Success",
                        "Your post has been updated successfully!",
                        "OK");

                    await Application.Current.MainPage.Navigation.PopModalAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        "Failed to update post in API",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Update Error",
                    $"Could not save changes: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Cancel()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}