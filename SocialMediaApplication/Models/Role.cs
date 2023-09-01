using System.ComponentModel.DataAnnotations;

namespace SocialMediaApplication.Models
{
    public class Role
    {
        [Key]
        [Required]
        public int roleId { get; set; }
        
        [Required]
        public string? roleName { get; set; } 
    }
}
