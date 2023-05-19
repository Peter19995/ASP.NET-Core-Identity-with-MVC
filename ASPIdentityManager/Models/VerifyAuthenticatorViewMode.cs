using System.ComponentModel.DataAnnotations;

namespace ASPIdentityManager.Models
{
    public class VerifyAuthenticatorViewMode
    {
        [Required]
        public string Code { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
