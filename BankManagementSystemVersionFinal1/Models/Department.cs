using System.ComponentModel.DataAnnotations;

namespace BankManagementSystemVersionFinal1.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        [Required]
        public string Name { get; set; }

        public List<Employee> EmployeesList { get; set; }

        public Department() { }
        public Department(string name)
        {
            EmployeesList = new List<Employee>();
            Name = name;
        }
        public void AddEmployeeToDepartment() { }
        public void RemoveEmployeeFromDepartment() { }
    }
}
