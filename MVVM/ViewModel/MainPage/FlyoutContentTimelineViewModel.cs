using System.Collections.ObjectModel;
using System.Windows.Input;

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
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand LikeCommand { get; }
        public ICommand EditPostCommand { get; }
        public ICommand DeletePostCommand { get; }

        public FlyoutContentTimelineViewModel()
        {
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

            MainThread.BeginInvokeOnMainThread( () =>
            {
                Posts.Clear();
                foreach ( var post in posts.OrderByDescending( p => DateTime.Parse( p.PostCreated ) ) )
                {
                    if ( string.IsNullOrWhiteSpace( post.FullName ) )
                        post.FullName = $"{post.FirstName} {post.LastName}";

                    Posts.Add( post );
                }

                IsRefreshing = false;
            } );
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
