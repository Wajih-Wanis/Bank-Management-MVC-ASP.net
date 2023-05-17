using BankManagementSystemVersionFinal1.Data;
using BankManagementSystemVersionFinal1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BankManagementSystemVersionFinal1.Controllers.Admin
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;




        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }



        [HttpGet("")]
        public IActionResult Index()
        {
            return View("~/Views/Admin-views/Index.cshtml");
        }




        /// employees section
        [HttpGet("employees")]
        public async Task<IActionResult> getEmployees()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.BankBranch)
                .ToListAsync();

            return View("~/Views/Admin-views/Employees/Index.cshtml", employees);
        }




        [HttpGet("employees/create")]
        public IActionResult createEmployee()
        {
            var bankBranches = _context.BankBranches.ToList();
            var departments = _context.Departments.ToList();
            //ViewBag.BankBranches = bankBranches;
            //ViewBag.Departments = departments;
            ViewBag.BranchList = new SelectList(bankBranches, nameof(BankBranch.BranchId), nameof(BankBranch.Name));
            ViewBag.DepartementList = new SelectList(departments, nameof(Department.DepartmentId), nameof(Department.Name));


            return View("~/Views/Admin-views/Employees/Create.cshtml");
        }




        [HttpPost("/admin/employees/create")]
        public async Task<IActionResult> createEmployeePostHandler([Bind("EmployeeId,Name,Address,Mail,PhoneNumber")] Employee employee, string role, int BranchId, int DepartemntId)
        {
            var BankBranch = _context.BankBranches.FirstOrDefault(b => b.BranchId == BranchId);
            var Department = _context.Departments.FirstOrDefault(d => d.DepartmentId == DepartemntId);
            // Create an instance of the appropriate derived class based on the selected role
            if (role == "Agent")
            {
                employee = new Agent(employee.Name, employee.Address, employee.PhoneNumber, Department, BankBranch, employee.Mail);
            }
            else if (role == "Manager")
            {
                employee = new Manager(employee.Name, employee.Address, employee.PhoneNumber, Department, BankBranch, employee.Mail);
            }

            // Set the BankBranch and Department properties based on the selected IDs
            

            // Save the employee to the database
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction("getEmployees");
        }


       

        [HttpGet("employees/details")]
        public async Task<IActionResult> getEmployeeDetails(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.BankBranch)
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin-views/Employees/Details.cshtml", employee);
        }



        [HttpGet("employees/edit")]
        public async Task<IActionResult> editEmployee(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            var bankBranches = _context.BankBranches.ToList();
            var departments = _context.Departments.ToList();
            //ViewBag.BankBranches = bankBranches;
            //ViewBag.Departments = departments;
            ViewBag.BranchList = new SelectList(bankBranches, nameof(BankBranch.BranchId), nameof(BankBranch.Name));
            ViewBag.DepartementList = new SelectList(departments, nameof(Department.DepartmentId), nameof(Department.Name));
            return View("~/Views/Admin-views/Employees/Edit.cshtml",employee);
        }


        
        [HttpPost("employees/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editEmployeePostHandler([Bind("EmployeeId,Name,Address,Mail,PhoneNumber")] Employee employee, int BranchId, int DepartemntId, string Role)
        {
            var bankBranch = _context.BankBranches.FirstOrDefault(b => b.BranchId == BranchId);
            var department = _context.Departments.FirstOrDefault(d => d.DepartmentId == DepartemntId);
            employee.BankBranch = bankBranch;
            employee.Department = department;
            var bankBranches = _context.BankBranches.ToList();
            var departments = _context.Departments.ToList();
            //ViewBag.BankBranches = bankBranches;
            //ViewBag.Departments = departments;
            ViewBag.BranchList = new SelectList(bankBranches, nameof(BankBranch.BranchId), nameof(BankBranch.Name));
            ViewBag.DepartementList = new SelectList(departments, nameof(Department.DepartmentId), nameof(Department.Name));
            _context.Update(employee);
            await _context.SaveChangesAsync();
            return View("~/Views/Admin-views/Employees/Edit.cshtml", employee);
        }



        [HttpGet("employees/delete")]
        public async Task<IActionResult> deleteEmployee(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin-views/Employees/Delete.cshtml", employee);
        }



        [HttpPost("employees/delete"), ActionName("deleteEmployee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> deleteEmployeeConfirmed(int EmployeeId)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Employees' is null.");
            }

            var employee = await _context.Employees.FindAsync(EmployeeId);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(getEmployees));
        }


        private bool EmployeeExists(int id)
        {
            return (_context.Employees?.Any(e => e.EmployeeId == id)).GetValueOrDefault();
        }


        // end employees section

        /// Bank Branch section

        [HttpGet("branches")]
        public async Task<IActionResult> getBankBranches()
        {
            var applicationDbContext = _context.BankBranches;
            return View("~/Views/Admin-views/BankBranches/Index.cshtml", await applicationDbContext.ToListAsync());
        }





        [HttpGet("branches/details")]
        public async Task<IActionResult> getBankBrancheDetails(int? id)
        {
            if (id == null || _context.BankBranches == null)
            {
                return NotFound();
            }

            var bankBranch = await _context.BankBranches
                .FirstOrDefaultAsync(m => m.BranchId == id);
            if (bankBranch == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin-views/BankBranches/Details.cshtml",bankBranch);
        }





        [HttpGet("branches/create")]
        public IActionResult createBankBranches()
        {
            return View("~/Views/Admin-views/BankBranches/Create.cshtml");
        }





        [HttpPost("branches/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BranchId,Name,ManagerId,City,PhoneNumber,BankBalance")] BankBranch bankBranch)
        {
                _context.Add(bankBranch);
                await _context.SaveChangesAsync();
                return RedirectToAction("getBankBranches");

        }




        [HttpGet("branches/edit")]
        public async Task<IActionResult> editBankBranche(int? id)
        {
            if (id == null || _context.BankBranches == null)
            {
                return NotFound();
            }

            var bankBranch = await _context.BankBranches.FindAsync(id);
            if (bankBranch == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin-views/BankBranches/Edit.cshtml",bankBranch);
        }



        [HttpPost("branches/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editBankBranchePostHandler([Bind("BranchId,Name,ManagerId,City,PhoneNumber,BankBalance")] BankBranch bankBranch)
        {

                    _context.Update(bankBranch);
                    await _context.SaveChangesAsync();

            return View("~/Views/Admin-views/BankBranches/Edit.cshtml", bankBranch);
        }





        [HttpGet("branches/delete")]
        public async Task<IActionResult> deleteBankBranche(int? id)
        {
            if (id == null || _context.BankBranches == null)
            {
                return NotFound();
            }

            var bankBranch = await _context.BankBranches.FindAsync(id);
            if (bankBranch == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin-views/BankBranches/Delete.cshtml",bankBranch);
        }





        [HttpPost("branches/delete"), ActionName("delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> deleteBankBranchConfirmed(int id, int BranchId)
        {
            if (_context.BankBranches == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BankBranches'  is null.");
            }
            var bankBranch = await _context.BankBranches.FindAsync(BranchId);
            if (bankBranch != null)
            {
                _context.BankBranches.Remove(bankBranch);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(getBankBranches));
        }




        private bool BankBranchExists(int id)
        {
          return (_context.BankBranches?.Any(e => e.BranchId == id)).GetValueOrDefault();
        }



        /// End bank branch section
        /// 


        // departements section start


        [HttpGet("departments")]
        public async Task<IActionResult> getDepartments()
        {
            return _context.Departments != null ?
                        View("~/Views/Admin-views/Departments/Index.cshtml", await _context.Departments.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Departments'  is null.");
        }




        [HttpGet("departments/details")]
        public async Task<IActionResult> getDepartmentDetails(int? id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.DepartmentId == id);
            if (department == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin-views/Departments/Details.cshtml", department);
        }





        [HttpGet("departments/create")]
        public IActionResult createDepartment()
        {
            return View("~/Views/Admin-views/Departments/Create.cshtml");
        }





        [HttpPost("departments/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> createDepartmentPostHandler([Bind("DepartmentId,Name")] Department department)
        {
            _context.Add(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(getDepartments));
        }





        [HttpGet("departments/edit")]
        public async Task<IActionResult> editDepartment(int? id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin-views/Departments/Edit.cshtml", department);
        }





        [HttpPost("departments/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editDepartmentPostHandler(int id, [Bind("DepartmentId,Name")] Department department)
        {
            _context.Update(department);
            await _context.SaveChangesAsync();
            return View("~/Views/Admin-views/Departments/Edit.cshtml", department);
        }





        [HttpGet("departments/delete")]
        public async Task<IActionResult> deleteDepartment(int? id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.DepartmentId == id);
            if (department == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin-views/Departments/Delete.cshtml", department);
        }






        [HttpPost("departments/delete"), ActionName("deleteDepartment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int DepartmentId)
        {
            if (_context.Departments == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Departments'  is null.");
            }
            var department = await _context.Departments.FindAsync(DepartmentId);
            if (department != null)
            {
                _context.Departments.Remove(department);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(getDepartments));
        }





        private bool DepartmentExists(int id)
        {
            return (_context.Departments?.Any(e => e.DepartmentId == id)).GetValueOrDefault();
        }



        // End departements section

    }
}
