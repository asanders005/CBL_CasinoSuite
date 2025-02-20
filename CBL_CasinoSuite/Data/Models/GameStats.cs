namespace CBL_CasinoSuite.Data.Models
{
    public class GameStats
    {
        public readonly string _GameName;
        public float TotalWins { get; private set; } = 0;
        public float TotalWinnings { get; private set; } = 0;
        public float TotalLosses { get; private set; } = 0;
        public float TotalLosings { get; private set; } = 0;

        public GameStats(string gameName)
        {
            _GameName = gameName;
        }

        public void Update(GameStats gameStats)
        {
            TotalWins = gameStats.TotalWins;
            TotalWinnings = gameStats.TotalWinnings;
            TotalLosses = gameStats.TotalLosses;
            TotalLosings = gameStats.TotalLosings;
        }
    }
}
