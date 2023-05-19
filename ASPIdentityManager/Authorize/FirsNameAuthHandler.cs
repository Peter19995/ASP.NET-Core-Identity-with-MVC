using ASPIdentityManager.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ASPIdentityManager.Authorize
{
    public class FirsNameAuthHandler : AuthorizationHandler<FirsNameAuthRequirement>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDBContext _db;
        public FirsNameAuthHandler(UserManager<IdentityUser> userManager,ApplicationDBContext db)
        {
            _userManager = userManager;
            _db = db;   
        }
        protected override  Task HandleRequirementAsync(AuthorizationHandlerContext context, FirsNameAuthRequirement requirement)
        {
            string userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _db.ApplicationUser.FirstOrDefault(a => a.Id == userId);
            var claims =Task.Run(async () => await _userManager.GetClaimsAsync(user)).Result;
            var claim  = claims.FirstOrDefault(c => c.Type == "FirstName");
            if(claim != null) {
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
