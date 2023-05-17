namespace BankManagementSystemVersionFinal1.Models
{
    public class Manager: Employee
    {
        public Manager() { }
        public Manager(string name, string address, int phoneNumber, Department department,BankBranch bankbranch, String mail) : base(name, address, phoneNumber, department, bankbranch,mail)
        { }
        public void ApproveLoan(Loan loan)// ikhali el etat loan approval
        {

        }
    }
}
