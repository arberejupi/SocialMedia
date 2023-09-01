using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization; 

namespace SocialMediaApplication.DTO
{
    public class UserDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int userId { get; set; }
        public string username { get; set; }
        public string name { get; set; } = string.Empty;
        public string surname { get; set; } = string.Empty;

        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string email { get; set; } = string.Empty;
        public string password { get; set; }
        public string roleName { get; set; }
        public string image { get; set; }
        public string Token { get; internal set; }
    }
}
