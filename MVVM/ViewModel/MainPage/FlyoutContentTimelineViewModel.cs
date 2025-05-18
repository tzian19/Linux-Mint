using System.Collections.ObjectModel;
using System.Windows.Input;

using Linux_Mint.MVVM.Model;

namespace Linux_Mint.MVVM.ViewModel
{
    public class FlyoutContentTimelineViewModel : ViewModelBase
    {
        private readonly PostService _postService;

        public ObservableCollection<UserPost> Posts { get; set; } = new();

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty( ref _isRefreshing , value );
        }

        public ICommand RefreshCommand { get; }
        public ICommand LikeCommand { get; }
        public ICommand EditPostCommand { get; }
        public ICommand DeletePostCommand { get; }

        public FlyoutContentTimelineViewModel( UserProfile loggedInUser )
        {
            LoggedInUser = loggedInUser;  // fixed assignment direction

            _postService = new PostService();

            RefreshCommand = new Command( async () => await LoadPostsAsync() );
            LikeCommand = new Command<UserPost>( LikePost );
            EditPostCommand = new Command<UserPost>( EditPost );
            DeletePostCommand = new Command<UserPost>( DeletePost );

            Task.Run( LoadPostsAsync );
        }

        private async Task LoadPostsAsync()
        {
            IsRefreshing = true;

            var posts = await _postService.GetPostsAsync();
            var users = await _postService.GetAllUsersAsync();

            var postList = posts
                .OrderByDescending(p =>
                {
                    DateTime.TryParse(p.PostCreated, out var parsedDate);
                    return parsedDate;
                })
                .ToList();

            MainThread.BeginInvokeOnMainThread( () =>
            {
                Posts.Clear();

                foreach ( var post in postList )
                {
                    var user = users.FirstOrDefault(u => u.UId == post.UserProfileId);
                    post.UserProfile = user ?? new UserProfile();

                    Console.WriteLine( $"Post by UserId: {post.UserProfileId}, Found: {user != null}, FullName: {post.UserProfile?.FullName}" );

                    Posts.Add( post );
                }

                IsRefreshing = false;
            } );

            //Testing
            foreach ( var user in users )
            {
                Console.WriteLine( $"User: {user.UId} - {user.FullName}" );
            }

            foreach ( var post in posts )
            {
                Console.WriteLine( $"Post.UserProfileId: {post.UserProfileId}" );
            }
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
