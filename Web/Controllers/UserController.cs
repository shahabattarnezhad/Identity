using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.ApplicationConsts;
using Web.Data;
using Web.Models;

namespace Web.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userList = _context.ApplicationUsers.ToList();
            var userRole = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();

            foreach (var user in userList)
            {
                var role = userRole.FirstOrDefault(u => u.UserId == user.Id);
                if (role is null)
                {
                    user.Role = "None";
                }
                else
                {
                    user.Role = roles.FirstOrDefault(r => r.Id == role.RoleId).Name;
                }
            }

            return View(userList);

        }

        [HttpGet]
        public IActionResult Edit(string userId)
        {
            var userFromDb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
            if (userFromDb == null)
            {
                return NotFound();
            }

            var userRole = _context.UserRoles.ToList();
            var roles = _context.Roles.ToList();
            var role = userRole.FirstOrDefault(u => u.UserId == userFromDb.Id);
            if (role is not null)
            {
                userFromDb.RoleId = roles.FirstOrDefault(r => r.Id == role.RoleId).Id;
            }

            userFromDb.RoleList = _context.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id
            });

            return View(userFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                var userFromDb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == applicationUser.Id);
                if (userFromDb == null)
                {
                    return NotFound();
                }

                var userRole = _context.UserRoles.FirstOrDefault(r => r.UserId == userFromDb.Id);
                if (userRole is not null)
                {
                    // In case of several role we need to use ToList()
                    //var previousRoleName = _context.Roles.Where(r => r.Id == userRole.RoleId)
                    //                                     .Select(x => x.Name)
                    //                                     .ToList();

                    // In this scenario we just have one role therefore we will use FirstOrDefault() 
                    var previousRoleName = _context.Roles.Where(r => r.Id == userRole.RoleId)
                                                         .Select(x => x.Name)
                                                         .FirstOrDefault();

                    // Removing the old role
                    await _userManager.RemoveFromRoleAsync(userFromDb, previousRoleName);
                }

                // Adding new role
                var newRole = _context.Roles.FirstOrDefault(r => r.Id == applicationUser.RoleId).Name;
                await _userManager.AddToRoleAsync(userFromDb, newRole);

                userFromDb.Name = applicationUser.Name;
                _context.SaveChanges();

                TempData[StaticDetails.Success] = "User has been edited successfully.";

                return RedirectToAction(nameof(Index));
            }


            applicationUser.RoleList = _context.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id
            });

            return View(applicationUser);
        }

        [HttpPost]
        public IActionResult LockUnlock(string userId)
        {
            var userFromDb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
            if(userFromDb is null)
            {
                return NotFound();
            }

            if(userFromDb.LockoutEnd is not null && userFromDb.LockoutEnd > DateTime.Now)
            {
                // User is locked and will keep the status untill the lockoutend time.
                // And clicking on this action will unlock them.

                userFromDb.LockoutEnd = DateTime.Now;
                TempData[StaticDetails.Success] = "User unlocked successfully.";
            }
            else
            {
                // User is not locked and is going to be locked.
                userFromDb.LockoutEnd = DateTime.Now.AddYears(100);
                TempData[StaticDetails.Success] = "User locked successfully.";
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(string userId)
        {
            var userFromDb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == userId);
            if (userFromDb is null)
            {
                return NotFound();
            }

            _context.ApplicationUsers.Remove(userFromDb);
            _context.SaveChanges();

            TempData[StaticDetails.Success] = "User deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
