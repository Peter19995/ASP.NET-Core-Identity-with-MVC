using System.ComponentModel.DataAnnotations;

namespace ASPIdentityManager.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
