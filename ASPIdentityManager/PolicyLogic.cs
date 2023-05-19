using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace ASPIdentityManager
{
    public static class PolicyLogic
    {
        public static bool AuthorizeAdminWithCalaimsOrSuperAdmin(AuthorizationHandlerContext context)
        {
            return (
            context.User.IsInRole("Admin") && context.User.HasClaim(c => c.Type == "Create" && c.Value == "True")
            && context.User.HasClaim(c => c.Type == "Edit" && c.Value == "True")
            && context.User.HasClaim(c => c.Type == "Delete" && c.Value == "True")) 
            || context.User.IsInRole("SuperAdmin");
        }
    }
}
