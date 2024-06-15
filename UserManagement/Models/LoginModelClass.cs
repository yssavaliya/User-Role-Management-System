using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models
{
    public class LoginModelClass
    {

        [Key]
        [ForeignKey("UserModeClass")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please enter username.")]
        [Column(TypeName = "nvarchar(10)")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Please enter password.")]
        [Column(TypeName = "nvarchar(100)")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "123";

        public UserModeClass UserModeClass { get; set; }
    }
}
