using CBL_CasinoSuite.Data.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CBL_CasinoSuite.Data.Models
{
    public class Dal : IDal
    {
        private static MongoClient dbClient = new MongoClient("mongodb+srv://verycoolmongoaccount:4!5$pVHyYSEb!L@users.sb3bg.mongodb.net/?retryWrites=true&w=majority&appName=Users");

        private static IMongoDatabase database = dbClient.GetDatabase("CasinoSuite");
        private static IMongoCollection<User> collection = database.GetCollection<User>("Users");

        public void AddUser(User user)
        {
            collection.InsertOne(user);
        }

        public void DeleteUser(string username)
        {
            collection.DeleteOne(u => u.Username.Equals(username));
        }

        public User GetUser(string username)
        {
            User foundUser = collection.AsQueryable().AsEnumerable().First(u => u.Username.Equals(username));
            //if (foundUser == null) foundUser = new User();
            return foundUser;
        }

        public User[] GetUsers()
        {
            return collection.AsQueryable().ToArray();
        }

        public void UpdateUser(string username, User user)
        {
            collection.AsQueryable().AsEnumerable().First(u => u.Username.Equals(username)).Update(user);
        }

        public void UpdateUserBalance(string username, float balance)
        {
            collection.AsQueryable().AsEnumerable().First(u => u.Username.Equals(username)).CurrentBalance = balance;
        }

        public void UpdateUserStatistics(string username, GameStats gameStatistics)
        {
            List<GameStats> userStats = collection.AsQueryable().AsEnumerable().First(u => u.Username.Equals(username)).GameStatistics;
            userStats.First(g => g._GameName.Equals(gameStatistics._GameName)).Update(gameStatistics);
        }

        List<User> IDal.GetUsers()
        {
            return collection.AsQueryable().ToList();
        }
    }
}
