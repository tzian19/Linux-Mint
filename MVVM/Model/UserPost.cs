using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Linux_Mint.MVVM.Model
{
    public class UserPost : INotifyPropertyChanged
    {
        public bool IsOwnPost => UserProfileId == AppState.LoggedInUser?.UId;
        public bool IsNotOwnPost => UserProfileId != AppState.LoggedInUser?.UId;

        public string PostId { get; set; }
        public string PostImage { get; set; }
        public string PostText { get; set; }
        public string PostCreated { get; set; }
        public string UserProfileId { get; set; }

        private List<string> _likedBy = new List<string>();
        public List<string> LikedBy
        {
            get => _likedBy;
            set
            {
                if ( _likedBy != value )
                {
                    _likedBy = value ?? new List<string>();
                    NotifyLikeChanged();
                }
            }
        }

        private List<string> _hiddenTo = new List<string>();
        public List<string> HiddenTo
        {
            get => _hiddenTo;
            set
            {
                if ( _hiddenTo != value )
                {
                    _hiddenTo = value ?? new List<string>();
                    NotifyHiddenChanged(); // FIX: Swapped this to the correct notifier
                }
            }
        }

        private string _currentUserId;
        public string CurrentUserId
        {
            get => _currentUserId;
            set
            {
                if ( _currentUserId != value )
                {
                    _currentUserId = value;
                    OnPropertyChanged();
                    OnPropertyChanged( nameof( IsLiked ) );
                    OnPropertyChanged( nameof( LikeIcon ) );

                    // NEW: Tell the UI to check if this specific user hid this post
                    OnPropertyChanged( nameof( IsHidden ) );
                    OnPropertyChanged( nameof( IsNotHidden ) );
                }
            }
        }

        public int LikeCount => LikedBy?.Count ?? 0;
        public bool IsLiked => LikedBy?.Contains( CurrentUserId ) ?? false;
        public string LikeIcon => IsLiked ? "❤️" : "🤍";

        // NEW: Helper properties for XAML bindings
        public bool IsHidden => HiddenTo?.Contains( CurrentUserId ) ?? false;
        public bool IsNotHidden => !IsHidden;

        private UserProfile _userProfile;
        public UserProfile UserProfile
        {
            get => _userProfile;
            set
            {
                _userProfile = value;
                OnPropertyChanged();
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

        public void OnPropertyChanged( [CallerMemberName] string name = null ) =>
            PropertyChanged?.Invoke( this , new PropertyChangedEventArgs( name ) );

        // --- LIKING LOGIC ---
        public void ToggleLike( string userId )
        {
            if ( string.IsNullOrEmpty( userId ) )
                return;
            if ( LikedBy == null )
                LikedBy = new List<string>();

            if ( LikedBy.Contains( userId ) )
                LikedBy.Remove( userId );
            else
                LikedBy.Add( userId );

            NotifyLikeChanged();
        }

        public void NotifyLikeChanged()
        {
            OnPropertyChanged( nameof( LikeCount ) );
            OnPropertyChanged( nameof( IsLiked ) );
            OnPropertyChanged( nameof( LikeIcon ) );
            OnPropertyChanged( nameof( LikedBy ) );
        }

        // --- NEW: HIDING LOGIC ---
        public void ToggleHide( string userId )
        {
            if ( string.IsNullOrEmpty( userId ) )
                return;
            if ( HiddenTo == null )
                HiddenTo = new List<string>();

            if ( HiddenTo.Contains( userId ) )
                HiddenTo.Remove( userId ); // Unhide
            else
                HiddenTo.Add( userId ); // Hide

            NotifyHiddenChanged();
        }

        public void NotifyHiddenChanged()
        {
            OnPropertyChanged( nameof( HiddenTo ) );
            OnPropertyChanged( nameof( IsHidden ) );
            OnPropertyChanged( nameof( IsNotHidden ) );
        }
    }
}