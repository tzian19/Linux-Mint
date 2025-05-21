using System.ComponentModel;

namespace Linux_Mint.MVVM.Model
{
    public class UserPost : INotifyPropertyChanged
    {
        public string PostId { get; set; }
        public string PostImage { get; set; }
        public string PostText { get; set; }
        public string PostCreated { get; set; }
        public string UserProfileId { get; set; }

        // New: List of user IDs who liked this post
        private List<string> _likedBy = new List<string>();
        public List<string> LikedBy
        {
            get => _likedBy;
            set
            {
                if ( _likedBy != value )
                {
                    _likedBy = value ?? new List<string>();
                    OnPropertyChanged( nameof( LikedBy ) );
                    OnPropertyChanged( nameof( LikeCount ) );
                    OnPropertyChanged( nameof( IsLiked ) );
                    OnPropertyChanged( nameof( LikeIcon ) );
                }
            }
        }

        // The current logged-in user's ID for checking if liked
        private string _currentUserId;
        public string CurrentUserId
        {
            get => _currentUserId;
            set
            {
                if ( _currentUserId != value )
                {
                    _currentUserId = value;
                    OnPropertyChanged( nameof( CurrentUserId ) );
                    OnPropertyChanged( nameof( IsLiked ) );
                    OnPropertyChanged( nameof( LikeIcon ) );
                }
            }
        }

        // LikeCount is derived from LikedBy count
        public int LikeCount => LikedBy?.Count ?? 0;

        // IsLiked depends on whether CurrentUserId is in LikedBy list
        public bool IsLiked => LikedBy?.Contains( CurrentUserId ) ?? false;

        // Update LikeIcon to show filled or outlined heart based on IsLiked
        public string LikeIcon => IsLiked ? "\uE800" : "\uE801";

        private UserProfile _userProfile;
        public UserProfile UserProfile
        {
            get => _userProfile;
            set
            {
                _userProfile = value;
                OnPropertyChanged( nameof( UserProfile ) );
                OnPropertyChanged( nameof( FullName ) );
                OnPropertyChanged( nameof( UserAvatar ) );
            }
        }

        public string FullName => UserProfile?.FullName ?? "Unknown User";

        public string UserAvatar => string.IsNullOrWhiteSpace( UserProfile?.UserAvatar )
            ? "default_avatar.png"
            : UserProfile.UserAvatar;

        public string TimeAgo
        {
            get
            {
                if ( !DateTime.TryParse( PostCreated , out var postTime ) )
                    return "";

                var span = DateTime.Now - postTime;

                if ( span.TotalSeconds < 0 )
                    return "0s";
                if ( span.TotalSeconds < 60 )
                    return $"{( int ) span.TotalSeconds}s";
                if ( span.TotalMinutes < 60 )
                    return $"{( int ) span.TotalMinutes}m";
                if ( span.TotalHours < 24 )
                    return $"{( int ) span.TotalHours}h";
                if ( span.TotalDays < 7 )
                    return $"{( int ) span.TotalDays}d";
                if ( span.TotalDays < 30 )
                    return $"{( int ) ( span.TotalDays / 7 )}w";

                return postTime.ToString( "MMM d, yyyy" );
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged( string name ) =>
            PropertyChanged?.Invoke( this , new PropertyChangedEventArgs( name ) );
    }
}
