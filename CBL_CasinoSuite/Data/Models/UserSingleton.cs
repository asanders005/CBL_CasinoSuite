using CBL_CasinoSuite.Data.Interfaces;

namespace CBL_CasinoSuite.Data.Models
{
    public class UserSingleton : IUser
    {
        private User _user = new User();

        public User GetUser()
        {
            return _user;
        }

        public void SetUser(User user)
        {
            _user = user;
        }
    }
}
