namespace Linux_Mint.MVVM.Model
{
    public class UserPost : UserProfile
    {
        public string UserID { get; set; }
        public string PostId { get; set; }
        public string FullName { get; set; }
        public string DatePosted { get; set; }
        public string ImageUrl { get; set; }
        public string PostText { get; set; }
        public int LikeCount { get; set; }

        public UserPost()
        {
            FullName = $"{FirstName} {LastName}";
        }
    }
}