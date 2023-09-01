namespace SocialMediaApplication.Models
{
    public class Comment
    {
        public int Id { get; set; }
       
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
