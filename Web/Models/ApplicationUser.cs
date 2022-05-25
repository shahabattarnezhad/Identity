using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Name is requiered.")]
        public string Name { get; set; }
    }
}
