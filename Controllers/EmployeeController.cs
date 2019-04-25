using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Northwind.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Northwind.Controllers
{
    //TODO: Add all the employee views

    public class EmployeeController : Controller
    {
        // this controller depends on the NorthwindRepository & the UserManager
        private INorthwindRepository repository;
        private UserManager<AppUser> userManager;
        public EmployeeController(INorthwindRepository repo, UserManager<AppUser> usrMgr)
        {
            repository = repo;
            userManager = usrMgr;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<IActionResult> Register(EmployeeWithPassword employeeWithPassword)
        {
            if (ModelState.IsValid)
            {
                Employees employee = employeeWithPassword.Employee;


                if (ModelState.IsValid)
                {
                    AppUser user = new AppUser
                    {
                        //TODO: try to set EmployeeID to AspNetUser's ID upon account creation
                        // email and username are synced - this is by choice
                        //Email = customer.Email,
                        //UserName = customer.Email
                    };
                    // Add user to Identity DB
                    IdentityResult result = await userManager.CreateAsync(user, employeeWithPassword.Password);
                    if (!result.Succeeded)
                    {
                        AddErrorsFromResult(result);
                    }
                    else
                    {
                        // Assign user to employee Role
                        result = await userManager.AddToRoleAsync(user, "Employee");

                        if (!result.Succeeded)
                        {
                            // Delete User from Identity DB
                            await userManager.DeleteAsync(user);
                            AddErrorsFromResult(result);
                        }
                        else
                        {
                            // Create employee (Northwind)
                            repository.AddEmployee(employee);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }

            }
            return View();
        }

        [Authorize(Roles = "Employee")]
        //TODO: Edit for employee
        //public IActionResult Account()
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        //    AppUser user = userManager.Users.FirstOrDefault(u => u.Id == userId);

        //    return View(repository.Employees.FirstOrDefault(e => e.EmployeeId == user.Id);
        //}

        [Authorize(Roles = "Employee"), HttpPost, ValidateAntiForgeryToken]
        public IActionResult Account(Employees employee)
        {
            // Edit employee info
            repository.EditEmployee(employee);
            return RedirectToAction("Index", "Home");
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
