using System.Collections.ObjectModel;
using System.Windows.Input;
using Linux_Mint.MVVM.Model;
using System.Threading.Tasks;

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
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand LikeCommand { get; }
        public ICommand EditPostCommand { get; }
        public ICommand DeletePostCommand { get; }

        public FlyoutContentTimelineViewModel()
        {
            _postService = new PostService();

            RefreshCommand = new Command(async () => await LoadPostsAsync());
            LikeCommand = new Command<UserPost>(LikePost);
            EditPostCommand = new Command<UserPost>(EditPost);
            DeletePostCommand = new Command<UserPost>(DeletePost);

            // Load on startup
            Task.Run(LoadPostsAsync);
        }

        private async Task LoadPostsAsync()
        {
            IsRefreshing = true;

            var posts = await _postService.GetPostsAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Posts.Clear();
                foreach (var post in posts.OrderByDescending(p => DateTime.Parse(p.PostCreated)))
                    Posts.Add(post);

                IsRefreshing = false;
            });
        }

        private void LikePost(UserPost post)
        {
            if (post == null) return;

            post.LikeCount++;
            // Here you could call the PostService to persist the like change if needed.
        }

        private void EditPost(UserPost post)
        {
            // Handle post editing logic
        }

        private void DeletePost(UserPost post)
        {
            if (post == null) return;

            Posts.Remove(post);
            // Here you could also call PostService to delete from backend if needed.
        }
    }
}
