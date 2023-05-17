using BankManagementSystemVersionFinal1.Data;
using BankManagementSystemVersionFinal1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Diagnostics;
namespace BankManagementSystemVersionFinal1.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var accounts = _context.Accounts
                .Include(a => a.AccountHolder)
                .Include(a => a.Transactions)
                .Include(a => a.loans)
                .Include(a => a.Transfers)
                .ToList();
            return View(accounts);

        }
        public async Task<IActionResult> Details(int id)
        {
            var account = await _context.Accounts
                .Include(a => a.AccountHolder)
                .FirstOrDefaultAsync(a => a.AccountId == id);


            if (account == null)
            {
                return NotFound();
            }

            var transactions = await _context.Transactions
                .Include(t => t.Account)
                .Where(t => t.Account.AccountId == id)
                .ToListAsync();


            var deposits = new List<Transaction>();
            var withdrawals = new List<Transaction>();

            foreach (var transaction in transactions)
            {


                if (transaction.Description.ToLower() == "deposit")
                {
                    deposits.Add(transaction);
                }
                else if (transaction.Description.ToLower() == "withdraw")
                {
                    withdrawals.Add(transaction);
                }
            }
            var loans = await _context.Loans
                .Include(l => l.Account)
                .Where(l => l.Account.AccountId == id)
                .ToListAsync();

            var transfers = await _context.Transfers
                .Include(t => t.Sender)
                    .ThenInclude(t => t.AccountHolder)
                .Include(t => t.Receiver)
                    .ThenInclude(t => t.AccountHolder)
                .Where(t => t.Sender.AccountId == id || t.Receiver.AccountId == id)
                .ToListAsync();

            ViewBag.Deposits = deposits;
            ViewBag.Withdrawals = withdrawals;
            ViewBag.loans = loans;
            ViewBag.transfers = transfers;
            ViewBag.account = account;

            return View(account);
        }

        // GET: Accounts/Create
        public IActionResult Create(int customerId)
        {
            ViewBag.CustomerId = customerId;
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // POST: Accounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int customerId, double balance, string TypeAccount)
        {

            var customer = _context.Customers.Find(customerId);

                if (customer == null)
                {
                    return NotFound();
                }
            Account account;
            if (TypeAccount == "CheckingAccount")
            {
                account = new CheckingAccount(customer, balance);
                _context.Accounts.Add(account as CheckingAccount);
            }
            else if (TypeAccount == "SavingAccount")
            {
                account = new SavingAccount(customer, balance);
                _context.Accounts.Add(account as SavingAccount);
            }
            else
            {
                Console.WriteLine(TypeAccount);

                return BadRequest("Invalid account type");
            }
            account.AccountStatus = true;

            account.AccountHolder = customer;

            _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Customers", new { id = customerId });
            
            //return View(account);
        }
        // GET: Accounts/Edit/5
        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                 .Include(a => a.AccountHolder)
                 .FirstOrDefaultAsync(a => a.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            ViewBag.CustomerId = account.AccountHolder.CustomerId;

            return View(account);
        }



        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,AccountStatus")] Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            try
            {
                var originalAccount = await _context.Accounts.Include(a => a.AccountHolder).FirstOrDefaultAsync(a => a.AccountId == id);

                if (originalAccount == null)
                {
                    return NotFound();
                }

                originalAccount.AccountStatus = account.AccountStatus;

                _context.Update(originalAccount);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Customers", new { id = originalAccount.AccountHolder.CustomerId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(account.AccountId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }



        //return View(account);
    

    // GET: Accounts/Delete/5
    public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountId == id);

            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.Include(a => a.AccountHolder)
                .FirstOrDefaultAsync(a => a.AccountId == id);
            var customerId = account.AccountHolder.CustomerId;  // Get CustomerId before deleting the account
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Customers", new { id = customerId });
        }

        private bool AccountExists(int id)
        {
            return (_context.Accounts?.Any(e => e.AccountId == id)).GetValueOrDefault();
        }




    }
}

