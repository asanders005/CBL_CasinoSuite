using CBL_CasinoSuite.Data.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
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
            collection.DeleteOne(u => u.Username == username);
        }

        public User GetUser(string username)
        {
            User foundUser = collection.Find(u => u.Username == username).FirstOrDefault();
            if (foundUser == null) foundUser = new User();
            return foundUser;
        }

        public User[] GetUsers()
        {
            return collection.AsQueryable().ToArray();
        }

        public void UpdateUser(string username, User user)
        {
            var update = Builders<User>.Update
                .Set(u => u.CurrentBalance, user.CurrentBalance)
                .Set(u => u.GameStatistics, user.GameStatistics);

            collection.UpdateOne(u => u.Username == username, update);
        }

        public void UpdateUserBalance(string username, float balance)
        {
            var update = Builders<User>.Update.Set(u => u.CurrentBalance, balance);
            collection.UpdateOne(u => u.Username == username, update);
        }

        public void UpdateUserPassword(string username, string password)
        {
            var update = Builders<User>.Update.Set(u => u.Password, password);
            collection.UpdateOne(u => u.Username == username, update);
        }

        public void UpdateUserStatistics(string username, GameStats gameStatistics)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.Username, username),
                Builders<User>.Filter.Eq("GameStatistics._GameName", gameStatistics._GameName) // Find the correct game
            );

            var update = Builders<User>.Update.Set("GameStatistics.$", gameStatistics);

            collection.UpdateOne(filter, update);
        }

        List<User> IDal.GetUsers()
        {
            return collection.AsQueryable().ToList();
        }
    }
}
