using MongoDB.Bson.Serialization.Attributes;

namespace CBL_CasinoSuite.Data.Models
{
    public class GameStats
    {
        private string gameName;

        [BsonElement("_gameName")]
        public string _GameName
        {
            get { return gameName; }
            set
            {
                if (gameName == null || gameName.Trim() == "")
                {
                    gameName = value;
                }
            }
        }

        [BsonElement("totalWins")]
        public int TotalWins { get; set; } = 0;

        [BsonElement("totalWinnings")]
        public float TotalWinnings { get; set; } = 0;

        [BsonElement("totalLosses")]
        public int TotalLosses { get; set; } = 0;

        [BsonElement("totalLosings")]
        public float TotalLosings { get; set; } = 0;

        public GameStats(string gameName)
        {
            _GameName = gameName;
        }
    }
}
