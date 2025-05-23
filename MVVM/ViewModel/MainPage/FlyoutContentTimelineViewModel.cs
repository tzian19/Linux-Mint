using System.Text;
using System.Text.Json;
using System.Windows.Input;

using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.View.MainPage;

namespace Linux_Mint.MVVM.ViewModel
{
    public class FlyoutContentTimelineViewModel : ViewModelBase
    {
        private readonly PostService _postService;
        private readonly string _currentUserId = "your-current-user-id"; // dynamically set this after login

        public ICommand AddNewPostCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand EditPostCommand { get; }
        public ICommand HidePostCommand { get; }
        public ICommand LikeCommand { get; }
        public ICommand ShowImageCommand { get; }

        public FlyoutContentTimelineViewModel()
        {
            _postService = new PostService();
            _currentUserId = LoggedInUser?.UId ?? string.Empty;

            ShowImageCommand = new Command<UserPost>( ShowImagePopup );
            AddNewPostCommand = new Command( async () => await NavigateToPage( new FlyoutContentNewPostView() ) );
            RefreshCommand = new Command( async () => await LoadPostsAsync() );
            EditPostCommand = new Command<UserPost>( EditPost );
            HidePostCommand = new Command<UserPost>( HidePost );
            LikeCommand = new Command<UserPost>( async ( post ) => await ToggleLikeAsync( post ) );

            Task.Run( LoadPostsAsync );
        }

        private async Task ToggleLikeAsync( UserPost post )
        {
            if ( post == null )
                return;

            // Initialize LikedBy if null
            if ( post.LikedBy == null )
                post.LikedBy = new List<string>();

            if ( post.IsLiked )
            {
                // Unlike: remove current user ID
                post.LikedBy.Remove( _currentUserId );
            }
            else
            {
                // Like: add current user ID
                post.LikedBy.Add( _currentUserId );
            }

            // Notify UI that these properties changed
            post.OnPropertyChanged( nameof( UserPost.LikedBy ) );
            post.OnPropertyChanged( nameof( UserPost.IsLiked ) );
            post.OnPropertyChanged( nameof( UserPost.LikeCount ) );
            post.OnPropertyChanged( nameof( UserPost.LikeIcon ) );

            var url = $"{baseUrl}/UserPosts/{post.PostId}";
            var json = JsonSerializer.Serialize(post, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(url, content);

            if ( !response.IsSuccessStatusCode )
            {
                // On failure, revert changes
                if ( post.IsLiked )
                    post.LikedBy.Remove( _currentUserId );
                else
                    post.LikedBy.Add( _currentUserId );

                post.OnPropertyChanged( nameof( UserPost.LikedBy ) );
                post.OnPropertyChanged( nameof( UserPost.IsLiked ) );
                post.OnPropertyChanged( nameof( UserPost.LikeCount ) );
                post.OnPropertyChanged( nameof( UserPost.LikeIcon ) );

                await Application.Current.MainPage.DisplayAlert( "Error" , "Failed to update like." , "OK" );
            }
        }

        private async Task LoadPostsAsync()
        {
            IsRefreshing = true;

            var users = await _postService.GetAllUsersAsync();
            var posts = await _postService.GetPostsAsync();

            var sortedPosts = posts
        .OrderByDescending(p =>
            DateTime.TryParse(p.PostCreated, out var parsedDate) ? parsedDate : DateTime.MinValue)
        .ToList();

            MainThread.BeginInvokeOnMainThread( () =>
            {
                Posts.Clear();

                foreach ( var post in sortedPosts )
                {
                    post.UserProfile = users.FirstOrDefault( u => u.UId == post.UserProfileId ) ?? new UserProfile();
                    post.CurrentUserId = _currentUserId;

                    if ( post.LikedBy == null )
                        post.LikedBy = new List<string>();

                    Posts.Add( post );
                }

                IsRefreshing = false;
            } );
        }


        private async void EditPost( UserPost post )
        {
            if ( post == null )
                return;

            var editPage = new EditPostPopup(post);
            await Application.Current.MainPage.Navigation.PushModalAsync( editPage );
        }

        private void HidePost( UserPost post )
        {
            if ( post == null )
                return;

            Posts.Remove( post );
        }
    }
}
