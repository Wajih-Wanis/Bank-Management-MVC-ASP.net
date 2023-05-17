using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BankManagementSystemVersionFinal1.Data;
using BankManagementSystemVersionFinal1.Models;

namespace BankManagementSystemVersionFinal1.Controllers
{
    public class TransfersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransfersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transfers
        public async Task<IActionResult> Index()
        {
            var transfers = _context.Transfers
               .Include(l => l.Sender)
               .ThenInclude(a => a.AccountHolder)
               .Include(l => l.Receiver)
               .ThenInclude(a => a.AccountHolder)
               .ToList();

            return View(transfers);
        }

        // GET: Transfers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Transfers == null)
            {
                return NotFound();
            }

            var transfer = await _context.Transfers
                .Include(t => t.Receiver)
                .Include(t => t.Sender)
                .FirstOrDefaultAsync(m => m.IdTransfer == id);
            if (transfer == null)
            {
                return NotFound();
            }

            return View(transfer);
        }

        // GET: Transfers/Create
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }


            var transfer = new Transfer { SenderId = id.Value };

            var sender = await _context.Accounts
           .Include(a => a.AccountHolder)
           .FirstOrDefaultAsync(a => a.AccountId == transfer.SenderId);

            if (sender == null)
            {
                return NotFound();
            }

            transfer.Sender = sender;



            return View(transfer);
        }

        // POST: Transfers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SenderId,ReceiverId,Amount")] Transfer transfer)
        {
            
            transfer.Sender = await _context.Accounts
            .Include(a => a.AccountHolder)
            .FirstOrDefaultAsync(a => a.AccountId == transfer.SenderId);

            if (transfer.Amount > transfer.Sender.Balance)
            {
                ModelState.AddModelError("Amount", "The transfer amount cannot exceed the account balance");
                return View(transfer);
            }

            transfer.Receiver = await _context.Accounts
            .OfType<CheckingAccount>() // Ensure that we only select CheckingAccounts
            .FirstOrDefaultAsync(a => a.AccountId == transfer.ReceiverId);

            
            if (transfer.Receiver == null)
            {
                // If the Receiver is still null, it means there was no CheckingAccount with the provided ID.
                ModelState.AddModelError("ReceiverId", "The Receiver Account ID must be a Checking Account");
                return View(transfer);
            }

            transfer.Sender.Balance -= transfer.Amount; // Deduct the transfer amount from sender's account balance
            transfer.Receiver.Balance += transfer.Amount; // Add the transfer amount to receiver's account balance


            transfer.Date = DateTime.Today;
            _context.Add(transfer);
            await _context.SaveChangesAsync();

            var accountHolderId = transfer.Sender.AccountHolder.CustomerId;
            return RedirectToAction("Details", "Client", new { id = accountHolderId });
        }

        

        private bool TransferExists(int id)
        {
          return (_context.Transfers?.Any(e => e.IdTransfer == id)).GetValueOrDefault();
        }
    }
}
