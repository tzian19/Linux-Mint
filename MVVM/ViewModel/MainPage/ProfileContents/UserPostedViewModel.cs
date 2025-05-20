using System.Windows.Input;

using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.View.MainPage.PopupPages;

namespace Linux_Mint.MVVM.ViewModel.MainPage.ProfileContents
{

    internal class UserPostedViewModel : ViewModelBase
    {
        private readonly PostService _postService;
        public ICommand RefreshCommand { get; }
        public ICommand LikeCommand { get; }
        public ICommand EditPostCommand { get; }
        public ICommand DeletePostCommand { get; }
        public ICommand ShowImageCommand { get; }
        public UserPostedViewModel()
        {
            ShowImageCommand = new Command<UserPost>( ShowImagePopup );
            _postService = new PostService();
            RefreshCommand = new Command( async () => await LoadCurrentUserPostsAsync() );
            LikeCommand = new Command<UserPost>( LikePost );
            EditPostCommand = new Command<UserPost>( EditPost );
            DeletePostCommand = new Command<UserPost>( DeletePost );
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

        private void EditPost( UserPost post )
        {
            // Implement popup edit logic here
        }

        private void DeletePost( UserPost post )
        {
            if ( post == null )
                return;

            Posts.Remove( post );
            // Optional: delete from backend
        }
    }
}
