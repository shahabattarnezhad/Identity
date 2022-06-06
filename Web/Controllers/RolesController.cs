using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web.Data;

namespace Web.Controllers
{
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(ApplicationDbContext context, 
                               UserManager<IdentityUser> userManager,
                               RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = _context.Roles.ToList();
            return View(roles);
        }

        [HttpGet]
        public IActionResult InsertUpdate(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                // Creation

                return View();
            }
            else
            {
                // Updation
                var objFromDb = _context.Roles.FirstOrDefault(r => r.Id == id);
                return View(objFromDb);
            }
        }
    }
}
