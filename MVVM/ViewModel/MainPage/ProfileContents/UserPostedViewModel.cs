using System.Collections.ObjectModel;
using System.Windows.Input;

using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.View.MainPage;

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

        public UserPostedViewModel()
        {
            _postService = new PostService();
            _currentUserId = LoggedInUser?.UId ?? string.Empty;

            ShowImageCommand = new Command<UserPost>( ShowImagePopup );
            EditPostCommand = new Command<UserPost>( EditPost );

            RefreshCommand = new Command( async () => await LoadUserPostsAsync() );
            LikeCommand = new Command<UserPost>( async post => await ToggleLikeAsync( post ) );

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
    }

}
