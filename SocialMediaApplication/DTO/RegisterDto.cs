using System.ComponentModel.DataAnnotations;

namespace SocialMediaApplication.DTO
{
    public class RegisterDto
    {
        public string? UserName { get; set; }//qetu u kan UserName
        public string? name { get; set; }
        public string? surname { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
        
        public string? Password { get; set; }
        public string? Role { get; set; }

    }
}
