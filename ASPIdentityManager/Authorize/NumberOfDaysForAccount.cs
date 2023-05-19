using ASPIdentityManager.Data;

namespace ASPIdentityManager.Authorize
{
    public class NumberOfDaysForAccount : INumberOfDaysForAccount
    {
        private readonly ApplicationDBContext _db;
        public NumberOfDaysForAccount(ApplicationDBContext db)
        {
            _db = db;
        }
        public int Get(string userId)
        {
           var user = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.DateCreated !=DateTime.MinValue)
            {
                return (DateTime.Today - user.DateCreated).Days;
            }
            return 0;
        }
    }
}
