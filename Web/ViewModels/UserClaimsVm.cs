using System.Collections.Generic;
using Web.Models;

namespace Web.ViewModels
{
    public class UserClaimsVm
    {
        public UserClaimsVm()
        {
            Claims = new List<UserClaim>();
        }

        public string UserId { get; set; }

        public List<UserClaim> Claims { get; set; }
    }
}
