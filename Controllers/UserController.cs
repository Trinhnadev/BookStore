using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FPTBook.Models;

namespace FPTBook.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            // Execute the first query to get the list of users
            var users = _userManager.Users.ToList();

            // Now, you can execute the second query to retrieve user roles
            var usersWithRoles = users.Select(user => new UserRoleViewModel
            {
                User = user,
                Roles = _userManager.GetRolesAsync(user).Result.ToList()
            }).ToList();

            return View(usersWithRoles);
        }

        public IActionResult Delete(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user != null)
            {
                var result = _userManager.DeleteAsync(user).Result;
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return View("Error");
        }

        public IActionResult ManageRoles(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(model: new UserRoleViewModel
            {
                User = user,
                Roles = roles
            });
        }

        [HttpPost]
        public IActionResult SetRoles(string id, List<string> Roles)
        {
            var user = _userManager.FindByIdAsync(id).Result;

            // Get the current user permissions
            var existingRoles = _userManager.GetRolesAsync(user).Result;

            // Check if the permission to assign exists for the user or not
            var rolesToAdd = Roles.Except(existingRoles).ToList();

            var result = _userManager.AddToRolesAsync(user, rolesToAdd).Result;
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
                return View("Error");
        }
    }
}