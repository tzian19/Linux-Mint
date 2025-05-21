using System.ComponentModel;

namespace Linux_Mint.MVVM.Model
{
    public class UserPost : INotifyPropertyChanged
    {
        public string PostId { get; set; }
        public string PostImage { get; set; }
        public string PostText { get; set; }
        public int LikeCount { get; set; }
        public string PostCreated { get; set; }
        public string UserProfileId { get; set; }

        private UserProfile _userProfile;

        public UserProfile UserProfile
        {
            get => _userProfile;
            set
            {
                _userProfile = value;
                OnPropertyChanged(nameof(UserProfile));
                OnPropertyChanged(nameof(FullName));
                OnPropertyChanged(nameof(UserAvatar));
            }
        }

        public string FullName => UserProfile?.FullName ?? "Unknown User";

        public string UserAvatar => string.IsNullOrWhiteSpace(UserProfile?.UserAvatar)
            ? "default_avatar.png"
            : UserProfile.UserAvatar;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        
    }
}