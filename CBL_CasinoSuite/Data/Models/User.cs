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

        private string username;

        [BsonElement("username")]
        public string Username { 
            get { return username; }
            set
            {
                if (username == null || username.Trim() == "")
                {
                    username = value;
                }
            }
        }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("currentBalance")]
        public float CurrentBalance { get; set; }

        [BsonElement("gameData")]
        public List<GameStats> GameStatistics { get; set; } = new();

        // variable used by Leaderboard model to determine placement on the leaderboard based on current filter type
        [BsonIgnore]
        public int LbPlacement { get; set; }
        [BsonIgnore]
        public EGameList LbFilter { get; set; } = EGameList.None;

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
    }
}
