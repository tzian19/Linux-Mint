using System.Collections.ObjectModel;
using System.Windows.Input;

using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.View.MainPage;
using Linux_Mint.MVVM.View.MainPage.PopupPages; // Needed for ImagePopupView
using Linux_Mint.Service;

namespace Linux_Mint.MVVM.ViewModel.MainPage.ProfileContents
{
    public class UserPostedViewModel : ViewModelBase
    {
        private readonly PostService _postService;
        private readonly string _currentUserId;

        public ObservableCollection<UserPost> Posts { get; } = new();

        public ICommand RefreshCommand { get; }
        public ICommand LikeCommand { get; }
        public ICommand EditPostCommand { get; }
        public ICommand ShowImageCommand { get; }
        public ICommand DeletePostCommand { get; }

        public UserPostedViewModel()
        {
            _postService = new PostService();
            _currentUserId = LoggedInUser?.UId ?? string.Empty;

            ShowImageCommand = new Command<UserPost>( ShowImagePopup );
            EditPostCommand = new Command<UserPost>( EditPost );

            RefreshCommand = new Command( async () => await LoadUserPostsAsync() );
            LikeCommand = new Command<UserPost>( async post => await ToggleLikeAsync( post ) );
            DeletePostCommand = new Command<UserPost>( async post => await DeletePostAsync( post ) );

            Task.Run( LoadUserPostsAsync );
        }

        public async Task LoadUserPostsAsync()
        {
            IsRefreshing = true;

            var allUsers = await _postService.GetAllUsersAsync();
            var allPosts = await _postService.GetPostsAsync();

            var userPosts = allPosts
                .Where(p => p.UserProfileId == _currentUserId)
                .OrderByDescending(p => DateTime.TryParse(p.PostCreated, out var d) ? d : DateTime.MinValue)
                .ToList();

            MainThread.BeginInvokeOnMainThread( () =>
            {
                Posts.Clear();

                foreach ( var post in userPosts )
                {
                    post.UserProfile = allUsers.FirstOrDefault( u => u.UId == post.UserProfileId ) ?? new UserProfile();
                    post.CurrentUserId = _currentUserId;

                    if ( post.LikedBy == null )
                        post.LikedBy = new List<string>();

                    Posts.Add( post );
                }

                IsRefreshing = false;
            } );
        }

        public async Task ToggleLikeAsync( UserPost post )
        {
            if ( post == null )
                return;

            if ( post.LikedBy == null )
                post.LikedBy = new List<string>();

            bool alreadyLiked = post.IsLiked;

            if ( alreadyLiked )
                post.LikedBy.Remove( _currentUserId );
            else
                post.LikedBy.Add( _currentUserId );

            post.NotifyLikeChanged();

            bool success = await _postService.UpdatePostAsync(post);
            if ( !success )
            {
                if ( alreadyLiked )
                    post.LikedBy.Add( _currentUserId );
                else
                    post.LikedBy.Remove( _currentUserId );

                post.NotifyLikeChanged();

                await Application.Current.MainPage.DisplayAlert( "Error" , "Failed to update like." , "OK" );
            }
        }

        private async void EditPost( UserPost post )
        {
            if ( post == null )
                return;

            var editPage = new EditPostPopup(post);
            await Application.Current.MainPage.Navigation.PushModalAsync( editPage );
        }

        private async Task DeletePostAsync( UserPost post )
        {
            if ( post == null || post.UserProfileId != _currentUserId )
            {
                await Application.Current.MainPage.DisplayAlert( "Permission Denied" , "You can only delete your own posts." , "OK" );
                return;
            }

            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirm Delete", "Are you sure you want to delete this post?", "Yes", "No");
            if ( !confirm )
                return;

            // FIX: Only passing the PostId now, matching the updated PostService
            bool deleted = await _postService.DeletePostAsync(post.PostId);

            if ( deleted )
            {
                MainThread.BeginInvokeOnMainThread( () => Posts.Remove( post ) );
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert( "Error" , "Failed to delete post." , "OK" );
            }
        }

        // FIX: Added the missing method for handling image clicks
        private async void ShowImagePopup( UserPost post )
        {
            if ( post == null || string.IsNullOrEmpty( post.PostImage ) )
                return;

            await Application.Current.MainPage.Navigation.PushModalAsync( new ImagePopupView( post.PostImage ) );
        }
    }
}