using CBL_CasinoSuite.Data.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CBL_CasinoSuite.Data.Models
{
    public class Dal : IDal
    {
        private static MongoClient dbClient = new MongoClient("mongodb+srv://verycoolmongoaccount:4!5$pVHyYSEb!L@users.sb3bg.mongodb.net/?retryWrites=true&w=majority&appName=Users");

        private static IMongoDatabase database = dbClient.GetDatabase("CasinoSuite");
        private static IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("Users");

        public void AddUser(User user)
        {
            var document = new BsonDocument {
                { "username", "Aiden" },
                { "password", "password" },
                { "total_balance", -1000000 },
                { "game_data", new BsonArray {
                    new BsonDocument {
                        { "name", "Blackjack" },
                        { "winnings", 3 },
                        { "losses", 9999999 },
                        { "win_count", 1 },
                        { "loss_count", 1 }
                    },
                  }
                }
            };

            collection.InsertOne(document);
        }

        public void DeleteUser(string username)
        {
            throw new NotImplementedException();
        }

        public User GetUser(string username)
        {
            throw new NotImplementedException();
        }

        public User[] GetUsers()
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

        List<User> IDal.GetUsers()
        {
            throw new NotImplementedException();
        }
    }
}
