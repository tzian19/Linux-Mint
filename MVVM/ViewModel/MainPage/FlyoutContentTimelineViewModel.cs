using System.Collections.ObjectModel;
using System.Windows.Input;

using Linux_Mint.MVVM.Model;
using Linux_Mint.Service;

namespace Linux_Mint.MVVM.ViewModel.MainPage
{
    public class FlyoutContentTimelineViewModel : ViewModelBase
    {
        private readonly UserService _userService;
        private ObservableCollection<UserPost> _posts;
        private UserProfile _currentUser;

        public ObservableCollection<UserPost> Posts
        {
            get => _posts;
            set { _posts = value; OnPropertyChanged(); }
        }

        public UserProfile CurrentUser
        {
            get => _currentUser;
            set { _currentUser = value; OnPropertyChanged(); }
        }

        public ICommand OnSwiped { get; }
        public ICommand ClickLike { get; }
        public ICommand EditDeletePostPopUp { get; }

        public FlyoutContentTimelineViewModel( UserProfile currentUser )
        {
            _userService = new UserService();
            CurrentUser = currentUser;
            OnSwiped = new Command<string>( async ( direction ) => await HandleSwipe( direction ) );
            ClickLike = new Command<UserPost>( async ( post ) => await LikePost( post ) );
            EditDeletePostPopUp = new Command<UserPost>( async ( post ) => await ShowPostEditDeleteOptions( post ) );

            LoadPosts();
        }

        private async void LoadPosts()
        {
            try
            {
                // Simulate loading posts from the service or mock data
                var userPosts = await _userService.GetUsersAsync(); // Assume this method retrieves posts related to the user
                Posts = new ObservableCollection<UserPost>( userPosts.Select( u => new UserPost
                {
                    PostId = u.UId ,
                    FullName = $"{u.FirstName} {u.LastName}" ,
                    ImageUrl = u.UserAvatar ,
                    DatePosted = DateTime.Now.ToString( "MMMM dd, yyyy" ) ,
                    LikeCount = new Random().Next( 1 , 100 ) // Mocked like count
                } ) );
            }
            catch ( Exception ex )
            {
                // Handle loading error
                Console.WriteLine( $"Error loading posts: {ex.Message}" );
            }
        }

        private async Task HandleSwipe( string direction )
        {
            if ( direction == "Right" )
            {
                // Handle right swipe (for example, navigate to a different page or show a context menu)
                Console.WriteLine( "Right swipe detected" );
                await Task.CompletedTask;
            }
        }

        private async Task LikePost( UserPost post )
        {
            // Simulate the action of liking a post
            post.LikeCount++;
            await Task.Delay( 500 ); // Simulate delay for the like action
            OnPropertyChanged( nameof( Posts ) ); // Refresh the list to update like count
        }

        private async Task ShowPostEditDeleteOptions( UserPost post )
        {
            // Simulate showing a pop-up to edit or delete the post
            bool action = await Application.Current.MainPage.DisplayAlert("Edit or Delete", "Do you want to edit or delete this post?", "Edit", "Delete");
            if ( action )
            {
                // Handle post edit action
                Console.WriteLine( $"Editing post: {post.FullName}" );
            }
            else
            {
                // Handle post delete action
                Console.WriteLine( $"Deleting post: {post.FullName}" );
            }
        }
    }
}
