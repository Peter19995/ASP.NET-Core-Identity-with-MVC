using System.ComponentModel.DataAnnotations;

namespace ASPIdentityManager.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        public string Email { get; set; }

        public string Name { get; set; }    
    }
}
