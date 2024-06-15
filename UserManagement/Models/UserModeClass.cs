    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace UserManagement.Models
    {
        public class UserModeClass
        {
            [Key]
            public int UserId { get; set; }

            [Required(ErrorMessage = "Please enter Name.")]
            [Column(TypeName = "nvarchar(30)")]

            public string Name { get; set; }

            [Required(ErrorMessage = "Please enter Email.")]
            [Column(TypeName = "nvarchar(100)")]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Please enter Gender.")]
            [Column(TypeName = "nvarchar(10)")]
            public string Gender { get; set; }

            [Required(ErrorMessage = "Please enter age.")]
            public int Age { get; set; }

            [Required(ErrorMessage = "Please enter city.")]
            [Column(TypeName = "nvarchar(20)")]
            public string City { get; set; }

            [Column(TypeName = "nvarchar(10)")]
            public string Role { get; set; }

            [Required(ErrorMessage = "Please enter username.")]
            [Column(TypeName = "nvarchar(10)")]
            public string? Username { get; set; }

            [Column(TypeName = "nvarchar(100)")]
            public string? profileImage { get; set; }
        }
    }
