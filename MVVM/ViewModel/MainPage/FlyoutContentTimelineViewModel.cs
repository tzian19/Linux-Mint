using System.Windows.Input;

using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.View.MainPage;

namespace Linux_Mint.MVVM.ViewModel
{
    public class FlyoutContentTimelineViewModel : ViewModelBase
    {
        private readonly PostService _postService;
        public ICommand AddNewPostCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand LikeCommand { get; }
        public ICommand EditPostCommand { get; }
        public ICommand HidePostCommand { get; }

        public FlyoutContentTimelineViewModel()
        {
            _postService = new PostService();

            AddNewPostCommand = new Command( async () => await NavigateToPage( new FlyoutContentNewPostView() ) );
            RefreshCommand = new Command( async () => await LoadPostsAsync() );
            LikeCommand = new Command<UserPost>( LikePost );
            EditPostCommand = new Command<UserPost>( EditPost );
            HidePostCommand = new Command<UserPost>( HidePost );

            Task.Run( LoadPostsAsync );
        }

        private async Task LoadPostsAsync()
        {
            IsRefreshing = true;

            var users = await _postService.GetAllUsersAsync();
            var posts = await _postService.GetPostsAsync();

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

        private async void EditPost( UserPost post )
        {
            if (post == null)
                return;



            var editPage = new EditPostPopup(post);
            await Application.Current.MainPage.Navigation.PushModalAsync(editPage);
        }

        private void HidePost( UserPost post )
        {

            if ( post == null )
                return;

            Posts.Remove( post );
        }
    }
}