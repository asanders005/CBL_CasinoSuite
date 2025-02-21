using CBL_CasinoSuite.Data.Interfaces;

namespace CBL_CasinoSuite.Data.Models
{
    public class DummyDB : IDal
    {
        private List<User> users = new List<User>();

        public DummyDB()
        {
            users.Add(new User("HelpMe", "password123", new GameList()));
        }

        public void AddUser(User user)
        {
            users.Add(user);
        }

        public void DeleteUser(string username)
        {
            throw new NotImplementedException();
        }

        public User GetUser(string username)
        {
            User user = users.FirstOrDefault(x => x.Username == username);
            if (user == null) return new User();
            return user;
        }

        public List<User> GetUsers()
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(string username, User user)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserBalance(string username, float balance)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserStatistics(string username, GameStats gameStatistics)
        {
            throw new NotImplementedException();
        }
    }
}
