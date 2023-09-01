namespace SocialMediaApplication.Models
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RequesterId { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
