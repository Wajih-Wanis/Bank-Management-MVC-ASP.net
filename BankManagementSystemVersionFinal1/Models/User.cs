using System.ComponentModel.DataAnnotations;

namespace BankManagementSystemVersionFinal1.Models
{
    public class User
    {
        [Key]
        public int LoginId { get; set; }
        [Required]

        public string Name { get; set; }
        public string Password { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
