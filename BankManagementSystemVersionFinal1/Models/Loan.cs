using System.ComponentModel.DataAnnotations;

namespace BankManagementSystemVersionFinal1.Models
{
    public class Loan
    {
        [Key]
        public int LoanId { get; set; }
        [Required]
        public Account Account { get; set; }
        public double? InterstRate { get; set; }
        public DateTime? StartingDate { get; set; }
        public List<Transaction> Transactions { get; set; }
        public DateTime? EndingDate { get; set; }
        public double Amount { get; set; }

        public enum LoanStatusEnum
        {
            Accepted,
            Refused,
            AwaitingApproval
        }
        public LoanStatusEnum LoanStatus { get; set; }

        public Loan() { }
        public Loan(CheckingAccount account, double ir, DateTime sd, DateTime ed, double amount, LoanStatusEnum loanStatus)
        {
            Account = account;
            InterstRate = ir;
            StartingDate = ed;
            Amount = amount;
            Transactions = new List<Transaction>();
            EndingDate = ed;
            LoanStatus = loanStatus;
        }
    }
}

