using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Data;

namespace Web.Authorize
{
    public class FirstNameAuthHandler : AuthorizationHandler<FirstNameAuthRequierment>
    {
        public UserManager<IdentityUser> _userManager { get; set; }
        public ApplicationDbContext _applicationDbContext { get; set; }

        public FirstNameAuthHandler(UserManager<IdentityUser> userManager,
                                    ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FirstNameAuthRequierment requirement)
        {
            string userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _applicationDbContext.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

            var claims = Task.Run(async () => await _userManager.GetClaimsAsync(user)).Result;
            var claim = claims.FirstOrDefault(c => c.Type == "FirstName");

            if(claim != null)
            {
                if (claim.Value.ToLower().Contains(requirement.Name.ToLower()))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }
    }
}
