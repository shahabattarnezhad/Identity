using System;
using System.Linq;
using Web.Data;

namespace Web.Authorize
{
    public class NumberOfDaysForAccount : INumberOfDaysForAccount
    {
        private readonly ApplicationDbContext _context;

        public NumberOfDaysForAccount(ApplicationDbContext context)
        {
            _context = context;
        }

        public int Get(string userId)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

            if(user is not null && user.CreatedDate != DateTime.MinValue)
            {
                var days = (DateTime.Today - user.CreatedDate).Days;
                return days;
            }

            return 0;
        }
    }
}
