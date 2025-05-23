using System.Windows.Input;

using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.View.MainPage;
using Linux_Mint.MVVM.View.MainPage.PopupPages;
using Linux_Mint.Service;

namespace Linux_Mint.MVVM.ViewModel
{
    public class FlyoutContentTimelineViewModel : ViewModelBase
    {
        private readonly PostService _postService;
        private readonly string _currentUserId;

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

            // UPDATED: Made this an async command
            HidePostCommand = new Command<UserPost>( async ( post ) => await HidePostAsync( post ) );

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

            // Use the PostService we created earlier!
            bool isSuccess = await _postService.UpdatePostAsync(post);

            if ( !isSuccess )
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
                // ADDED THIS FILTER: Skip any posts where the HiddenTo list contains the current user's ID
                .Where(p => p.HiddenTo == null || !p.HiddenTo.Contains(_currentUserId))
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

        // UPDATED: Added Confirmation, Database Save, and UI Removal
        private async Task HidePostAsync( UserPost post )
        {
            if ( post == null )
                return;

            // 1. Show Confirmation Dialog
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Hide Post",
                "Are you sure you want to hide this post? It will no longer appear in your timeline.",
                "Yes, Hide",
                "Cancel");

            if ( confirm )
            {
                // 2. Add current user ID to the post's HiddenTo list
                post.ToggleHide( _currentUserId );

                // 3. Remove from UI immediately so it vanishes seamlessly
                Posts.Remove( post );

                // 4. Save the new hidden status to MockAPI
                bool success = await _postService.UpdatePostAsync(post);

                if ( !success )
                {
                    Console.WriteLine( "❌ API ERROR: Failed to save hidden post state to database." );
                }
            }
        }

        private async void ShowImagePopup( UserPost post )
        {
            if ( post == null || string.IsNullOrEmpty( post.PostImage ) )
                return;

            await Application.Current.MainPage.Navigation.PushModalAsync( new ImagePopupView( post.PostImage ) );
        }
    }
}