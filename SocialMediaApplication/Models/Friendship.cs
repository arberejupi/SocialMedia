using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMediaApplication.Models
{
    public class Friendship
    {
        [Key]
        public int RequestId { get; set; } // Primary Key
        [ForeignKey("UserId")]
        public int SenderId { get; set; } // Foreign Key to Users Table
        [ForeignKey("FriendId")]
        public int ReceiverId { get; set; } // Foreign Key to Users Table
        public string Status { get; set; } // Status: "Pending" or "Accepted"
        public DateTime CreatedAt { get; set; } // Timestamp
    }
}
