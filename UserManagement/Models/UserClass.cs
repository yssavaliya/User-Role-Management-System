using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Models
{
    public class UserClass
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please enter Name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter Email.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter Gender.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Please enter age.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Please enter city.")]
        public string City { get; set; }

        public string Role { get; set; }

        [Required(ErrorMessage = "Please enter username.")]
        public string? Username { get; set; } 

        public IFormFile profileImage { get; set; }
    }
}
