using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankManagementSystemVersionFinal1.Models
{
    public class Transfer
    {
        [Key]
        public int IdTransfer { get; set; }

        public int SenderId { get; set; }
        [ForeignKey("SenderId")]
        public Account Sender { get; set; }


        public int ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public CheckingAccount Receiver { get; set; }

        public DateTime Date { get; set; }

        public double Amount { get; set; }

        public Transfer() { }
        public Transfer(CheckingAccount sender, CheckingAccount reciever, double amount)
        {
            Sender = sender;
            Receiver = reciever;
            Amount = amount;
            Date = DateTime.Now;
        }

        public void Commit() { }

    }
}
