using Microsoft.AspNetCore.Authorization;

namespace ASPIdentityManager.Authorize
{
    public class FirsNameAuthRequirement : IAuthorizationRequirement
    {

        public FirsNameAuthRequirement(string name)
        {
            Name = name;
        }

        public string Name { get; set; }    
    }
}
