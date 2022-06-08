using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Web.ApplicationConsts;
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

        [HttpPost]
        [Authorize(Policy = "OnlySuperAdminChecker")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertUpdate(IdentityRole identityRole)
        {
            if(await _roleManager.RoleExistsAsync(identityRole.Name))
            {
                // throw an error
                TempData[StaticDetails.Error] = "Role already exists.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrEmpty(identityRole.Id))
            {
                // Creation
                await _roleManager.CreateAsync(new IdentityRole() { Name = identityRole.Name });

                TempData[StaticDetails.Success] = "Role created successfully.";
            }
            else
            {
                // Updation
                var roleObjFromDb = await _context.Roles.FirstOrDefaultAsync(r => r.Id == identityRole.Id);

                if(roleObjFromDb is null)
                {
                    TempData[StaticDetails.Error] = "Role not found.";
                    return RedirectToAction(nameof(Index));
                }

                roleObjFromDb.Name = identityRole.Name;
                roleObjFromDb.NormalizedName = identityRole.Name.ToUpper();

                var result = await _roleManager.UpdateAsync(roleObjFromDb);

                TempData[StaticDetails.Success] = "Role updated successfully.";
            }

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        [Authorize(Policy = "OnlySuperAdminChecker")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var objFromDb = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (objFromDb is null)
            {
                TempData[StaticDetails.Error] = "Role not found.";
                return RedirectToAction(nameof(Index));
            }

            var relatedUserRoles = _context.UserRoles.Where(u => u.RoleId == id).Count();
            if(relatedUserRoles > 0)
            {
                TempData[StaticDetails.Error] = "Unable to delete this role, some users are in relation.";
                return RedirectToAction(nameof(Index));
            }

            await _roleManager.DeleteAsync(objFromDb);

            TempData[StaticDetails.Success] = "Role deleted successfully.";
            return RedirectToAction(nameof(Index));

        }
    }
}
