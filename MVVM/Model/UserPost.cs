using Microsoft.VisualStudio.PlatformUI;
using System.ComponentModel;

namespace Linux_Mint.MVVM.Model
{
    public class UserPost : INotifyPropertyChanged
    {
        public string? PostId { get; set; }
        public string? PostImage { get; set; }
        public string? PostText { get; set; }
        public int LikeCount { get; set; }
        public string? PostCreated { get; set; }
        public string? UserProfileId { get; set; }

        public bool IsOwnPost => UserProfileId == AppState.LoggedInUser?.UId;
        public bool IsNotOwnPost => UserProfileId != AppState.LoggedInUser?.UId;

        private UserProfile? _userProfile;

        public UserProfile? UserProfile
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




        public string FullName => UserProfile?.FullName ?? "Unknown User";

        public string UserAvatar => string.IsNullOrWhiteSpace( UserProfile?.UserAvatar )
            ? "default_avatar.png"
            : UserProfile.UserAvatar;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

     
    }
}