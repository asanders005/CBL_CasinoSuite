using CBL_CasinoSuite.Data.Models;

namespace CBL_CasinoSuite.Data.Interfaces
{
    public interface IUser
    {
        public User GetUser();
        public void SetUser(User user);
    }
}
