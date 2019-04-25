using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Northwind.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Northwind.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // dependencies required
        private IUserValidator<AppUser> appUserValidator;
        private IPasswordValidator<AppUser> appPasswordValidator;
        private IPasswordHasher<AppUser> appPasswordHasher;
        private UserManager<AppUser> appUserManager;
        private IUserValidator<EmployeeUser> employeeUserValidator;
        private IPasswordValidator<EmployeeUser> employeePasswordValidator;
        private IPasswordHasher<EmployeeUser> employeePasswordHasher;
        private UserManager<EmployeeUser> employeeUserManager;

        // inject dependencies in constructor
        public AdminController(UserManager<AppUser> usrMgr, IUserValidator<AppUser> userValid, 
            IPasswordValidator<AppUser> passValid, IPasswordHasher<AppUser> passwordHash, 
            UserManager<EmployeeUser> empMgr, IUserValidator<EmployeeUser> employeeValid, 
            IPasswordValidator<EmployeeUser> empPassValid, IPasswordHasher<EmployeeUser> empPasswordHash)
        {
            appUserManager = usrMgr;
            appUserValidator = userValid;
            appPasswordValidator = passValid;
            appPasswordHasher = passwordHash;
            employeeUserManager = empMgr;
            employeeUserValidator = employeeValid;
            employeePasswordValidator = empPassValid;
            employeePasswordHasher = empPasswordHash;
        }

        public ViewResult Index() => View(new AllUsersModel { AppUsers = appUserManager.Users, EmployeesUsers = employeeUserManager.Users });

        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeModel model)
        {
            if (ModelState.IsValid)
            {
                EmployeeUser user = new EmployeeUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };
                IdentityResult result = await employeeUserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await appUserManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await appUserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View("Index", appUserManager.Users);
        }

        // Edit method accessed via HttpGet accepts a string for the id, which is passed by the tag helper asp-route-id
        public async Task<IActionResult> Edit(string id)
        {
            AppUser user = await appUserManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // Edit method accessed via HTtpPost accepts the string id, string email, and string password
        // validation on user input is validated here
        [HttpPost]
        public async Task<IActionResult> Edit(string id, string email, string password)
        {
            // Get the user by id
            AppUser user = await appUserManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Email = email;
                IdentityResult validEmail = await appUserValidator.ValidateAsync(appUserManager, user);
                if (!validEmail.Succeeded)
                {
                    AddErrorsFromResult(validEmail);
                }
                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(password))
                {
                    validPass = await appPasswordValidator.ValidateAsync(appUserManager, user, password);
                    if (validPass.Succeeded)
                    {
                        user.PasswordHash = appPasswordHasher.HashPassword(user, password);
                    }
                    else
                    {
                        AddErrorsFromResult(validPass);
                    }
                }
                if ((validEmail.Succeeded && validPass == null) || (validEmail.Succeeded && password != string.Empty && validPass.Succeeded))
                {
                    IdentityResult result = await appUserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(user);
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
