using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class VerifyAuthenticatorVm
    {
        [Required]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
