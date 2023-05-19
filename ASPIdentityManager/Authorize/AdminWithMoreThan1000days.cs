using Microsoft.AspNetCore.Authorization;

namespace ASPIdentityManager.Authorize
{
    public class AdminWithMoreThan1000days : IAuthorizationRequirement
    {
        public AdminWithMoreThan1000days(int days)
        {
            Days = days; 
        }
        public int Days { get; set; }   
    }
}
