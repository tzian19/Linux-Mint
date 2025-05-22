using System.Text;
using System.Text.Json;
using System.Windows.Input;

using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.View.MainPage;

namespace Linux_Mint.MVVM.ViewModel.MainPage.ProfileContents
{
    internal class UserPostedViewModel : ViewModelBase
    {
        private readonly PostService _postService;
        public UserPost _originalPost;
        private readonly string _currentUserId;

        public ICommand RefreshCommand { get; }
        public ICommand LikeCommand { get; }
        public ICommand EditPostCommand { get; }
        public ICommand DeletePostCommand { get; }
        public ICommand ShowImageCommand { get; }
        public bool IsBusy { get; private set; }
        
        public UserPostedViewModel()
        {
            _postService = new PostService();
            _currentUserId = LoggedInUser?.UId ?? string.Empty;

            ShowImageCommand = new Command<UserPost>( ShowImagePopup );
            RefreshCommand = new Command( async () => await LoadCurrentUserPostsAsync() );
            LikeCommand = new Command<UserPost>( async ( post ) => await ToggleLikeAsync( post ) );
            EditPostCommand = new Command<UserPost>( EditPost );
            DeletePostCommand = new Command<UserPost>(async (post) => await DeletePost(post));
            
            Task.Run( LoadCurrentUserPostsAsync );
        }

        private async Task ToggleLikeAsync( UserPost post )
        {
            if ( post == null || string.IsNullOrEmpty( _currentUserId ) )
                return;

            if ( post.LikedBy == null )
                post.LikedBy = new List<string>();

            bool wasLiked = post.IsLiked;

            if ( wasLiked )
                post.LikedBy.Remove( _currentUserId );
            else
                post.LikedBy.Add( _currentUserId );

            // Notify property changed correctly
            post.NotifyLikeChanged();

            try
            {
                var url = $"{baseUrl}/UserPosts/{post.PostId}";
                var json = JsonSerializer.Serialize(post, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(url, content);

                if ( !response.IsSuccessStatusCode )
                    throw new Exception( "Server returned error" );
            }
            catch ( Exception )
            {
                // revert changes on failure
                if ( wasLiked )
                    post.LikedBy.Add( _currentUserId );
                else
                    post.LikedBy.Remove( _currentUserId );

                post.NotifyLikeChanged();

                await Application.Current.MainPage.DisplayAlert( "Error" , "Failed to update like." , "OK" );
            }
        }

        private async Task LoadCurrentUserPostsAsync()
        {
            IsRefreshing = true;

            try
            {
                var loggedInUserId = LoggedInUser?.UId;
                if ( string.IsNullOrEmpty( loggedInUserId ) )
                    return;

                var posts = await _postService.GetPostsAsync();

                var userPosts = posts
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
                        post.UserProfile = LoggedInUser;
                        post.CurrentUserId = _currentUserId;

                        if ( post.LikedBy == null )
                            post.LikedBy = new List<string>();

                        Posts.Add( post );
                    }
                } );
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"Failed to load posts: {ex.Message}" );
                await Application.Current.MainPage.DisplayAlert( "Error" , "Unable to load posts." , "OK" );
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async void EditPost( UserPost post )
        {
            if ( post == null )
                return;

            // Verify the post belongs to the current user
            if (post.UserProfileId != AppState.LoggedInUser.UId)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "You can only edit your own posts", "OK");
                return;
            }

            var editPage = new EditPostPopup(post);
            await Application.Current.MainPage.Navigation.PushModalAsync( editPage );
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
