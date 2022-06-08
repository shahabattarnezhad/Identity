using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class AccessCheckerController : Controller
    {
        public IActionResult AllAccess()
        {
            return View();
        }

        [Authorize]
        public IActionResult AuthorizedAccess()
        {
            return View();
        }

        [Authorize(Roles = "User")]
        public IActionResult UserAccess()
        {
            return View();
        }

        // This is OR condition
        [Authorize(Roles = "User, Admin")]
        public IActionResult UserOrAdminAccess()
        {
            return View();
        }

        // This is AND condition
        [Authorize(Policy = "UserAndAdmin")]
        public IActionResult UserAndAdminAccess()
        {
            return View();
        }

        [Authorize(Policy = "Admin")]
        public IActionResult AdminAccess()
        {
            return View();
        }

        [Authorize(Policy = "Admin_CreateAccess")]
        public IActionResult Admin_CreateAccess()
        {
            return View();
        }

        [Authorize(Policy = "Admin_Create_Edit_DeleteAccess")]
        public IActionResult Admin_Create_Edit_DeleteAccess()
        {
            return View();
        }

        [Authorize(Policy = "Admin_Create_Edit_DeleteAccess_Or_SuperAdmin")]
        public IActionResult Admin_Create_Edit_DeleteAccess_Or_SuperAdmin()
        {
            return View();
        }

        [Authorize(Policy = "AdminWithMoreThan1000Days")]
        public IActionResult SpecialPage()
        {
            return View();
        }
    }
}
