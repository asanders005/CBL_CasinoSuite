using CBL_CasinoSuite.Data.Models;

namespace CBL_CasinoSuite.Data.Interfaces
{
    public interface IDal
    {
        public User GetUser(string username);
        public User[] GetUsers();
        public void AddUser(User user);
        public void UpdateUser(string username, User user);
        public void DeleteUser(string username);
    }
}
