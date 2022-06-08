using Microsoft.AspNetCore.Authorization;

namespace Web.Authorize
{
    public class AdminWithMoreThan1000DaysRequierment : IAuthorizationRequirement
    {
        public AdminWithMoreThan1000DaysRequierment(int days)
        {
            Days = days;
        }

        public int Days { get; set; }
    }
}
