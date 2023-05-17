using System.ComponentModel.DataAnnotations;

namespace BankManagementSystemVersionFinal1.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }

        public string Mail { get; set; }
        public int PhoneNumber { get; set; }
        public Department Department { get; set; }

        public BankBranch BankBranch { get; set; }

        public Employee() { }
        public Employee(string name, string address, int phoneNumber, Department department, BankBranch bankbranch, string mail)
        {

            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
            Department = department;
            BankBranch = bankbranch;
            Mail = mail;
        }
    }
}
