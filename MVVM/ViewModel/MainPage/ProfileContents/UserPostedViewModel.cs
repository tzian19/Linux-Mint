using System.Windows.Input;

using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.View.MainPage.PopupPages;
using Linux_Mint.MVVM.View.MainPage;

namespace Linux_Mint.MVVM.ViewModel.MainPage.ProfileContents
{

    internal class UserPostedViewModel : ViewModelBase
    {
        private readonly PostService _postService;
        public UserPost _originalPost;

        public ICommand RefreshCommand { get; }
        public ICommand LikeCommand { get; }
        public ICommand EditPostCommand { get; }
        public ICommand DeletePostCommand { get; }
        public ICommand ShowImageCommand { get; }
        public bool IsBusy { get; private set; }

        public UserPostedViewModel()
        {
            ShowImageCommand = new Command<UserPost>( ShowImagePopup );
            _postService = new PostService();
            RefreshCommand = new Command( async () => await LoadCurrentUserPostsAsync() );
            LikeCommand = new Command<UserPost>( LikePost );
            EditPostCommand = new Command<UserPost>( EditPost );
            DeletePostCommand = new Command<UserPost>(async (post) => await DeletePost(post));
            Task.Run( LoadCurrentUserPostsAsync );
        }
        private async void ShowImagePopup( UserPost post )
        {
            if ( post == null || string.IsNullOrEmpty( post.PostImage ) )
                return;

            await Application.Current.MainPage.Navigation.PushModalAsync( new ImagePopupView( post.PostImage ) );
        }
        private async Task LoadCurrentUserPostsAsync()
        {
            IsRefreshing = true;

            var loggedInUserId = AppState.LoggedInUser.UId;

            var users = await _postService.GetAllUsersAsync();
            var allPosts = await _postService.GetPostsAsync();

            // Filter posts for current user
            var userPosts = allPosts
        .Where(p => p.UserProfileId == loggedInUserId)
        .OrderByDescending(p =>
        {
            DateTime.TryParse(p.PostCreated, out var parsedDate);
            return parsedDate;
        })
        .ToList();

            MainThread.BeginInvokeOnMainThread( () =>
            {
                Posts.Clear();

                foreach ( var post in userPosts )
                {
                    post.UserProfile = AppState.LoggedInUser; // Set directly since it's the current user
                    Posts.Add( post );
                }

                IsRefreshing = false;
            } );

            // Optional: Debug logging
            Console.WriteLine( $"Logged-in User ID: {loggedInUserId}" );
            Console.WriteLine( $"User has {userPosts.Count} posts." );
        }
        private void LikePost( UserPost post )
        {
            if ( post == null )
                return;

            post.LikeCount++;
            OnPropertyChanged( nameof( Posts ) );
        }

        private async void EditPost( UserPost post )
        {
            if (post == null)
                return;

            // Verify the post belongs to the current user
            if (post.UserProfileId != AppState.LoggedInUser.UId)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "You can only edit your own posts", "OK");
                return;
            }

            var editPage = new EditPostPopup(post);
            await Application.Current.MainPage.Navigation.PushModalAsync(editPage);
        }

        private async Task DeletePost(UserPost post)
        {
            bool success = await _postService.DeletePostAsync(post);

            if (success)
            {
                // Show success message before closing
                await Application.Current.MainPage.DisplayAlert(
                    "Success",
                    "Your post has been deleted successfully!",
                    "OK");

                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Failed to delete post in API",
                    "OK");
            }
        }




       
    }
}
