using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class ForgotPasswordVm
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
