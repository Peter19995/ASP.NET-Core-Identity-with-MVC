using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ASPIdentityManager.Authorize
{
    public class AdminWithOver1000DaysHandler : AuthorizationHandler<AdminWithMoreThan1000days>
    {
        private readonly INumberOfDaysForAccount _numberOfDaysForAccount;
        public AdminWithOver1000DaysHandler(INumberOfDaysForAccount numberOfDaysForAccount)
        {
            _numberOfDaysForAccount = numberOfDaysForAccount;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminWithMoreThan1000days requirement)
        {
            if (!context.User.IsInRole("Admin"))
            {
                return Task.CompletedTask;
            }
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int numberOfDays = _numberOfDaysForAccount.Get(userId);
            if (numberOfDays >= requirement.Days) {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
