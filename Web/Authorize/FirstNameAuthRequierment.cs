using Microsoft.AspNetCore.Authorization;

namespace Web.Authorize
{
    public class FirstNameAuthRequierment : IAuthorizationRequirement
    {
        public FirstNameAuthRequierment(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
