namespace Web.ViewModels
{
    public class TwoFactorAuthenticationVm
    {
        public string Code { get; set; }

        public string Token { get; set; }

        public string QRCodeUrl { get; set; }
    }
}
