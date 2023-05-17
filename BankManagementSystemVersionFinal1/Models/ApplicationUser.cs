using Microsoft.AspNetCore.Identity;

namespace BankManagementSystemVersionFinal1.Models
{
    public class ApplicationUser: IdentityUser
    {
        public String FirstName { get; set; }
        public string LastName { get; set; }
    }
}
