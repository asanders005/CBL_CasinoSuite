using CBL_CasinoSuite.Data.Interfaces;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace CBL_CasinoSuite.Data.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("username")]
        public readonly string Username;

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("currentBalance")]
        public float CurrentBalance { get; set; }

        [BsonElement("gameData")]
        [BsonIgnoreIfNull]
        public List<GameStats> GameStatistics { get; set; } = new List<GameStats>();

        public User()
        {
            Username = "";
        }

        public User(string username, string pass, IGameList gameList)
        {
            Username = username;
            Password = pass;
            CurrentBalance = 500f;

            foreach (var game in gameList.GetGameList())
            {
                GameStatistics.Add(new GameStats(game));
            }
        }

        public void Update(User user)
        {
            CurrentBalance = user.CurrentBalance;
            GameStatistics = user.GameStatistics;
        }
    }
}
