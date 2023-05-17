namespace BankManagementSystemVersionFinal1.Models
{
    public class SavingAccount: Account
    {
        static double InterstRate = 0.07;

        public SavingAccount() { }
        public SavingAccount(Customer customer, double balance) : base(customer, balance)
        {
        }

        public void AddInterst() { }

    }
}
