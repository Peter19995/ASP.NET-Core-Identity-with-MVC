namespace ASPIdentityManager.Models
{
    public class TwoFactorAuthenticationViewModel
    {
        //used To Register
        public string Code { get; set; }


        //Used To Login
        public string Token { get; set; }

        public string? QRCodeUrl { get; set; }
    }
}
