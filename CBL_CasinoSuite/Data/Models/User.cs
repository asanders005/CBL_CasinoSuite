using CBL_CasinoSuite.Data.Interfaces;

namespace CBL_CasinoSuite.Data.Models
{
    public class User
    {
        public readonly string Username;
        public string Password { get; set; } = "";
        public float CurrentBalance { get; private set; } = -1;
        public List<GameStats> GameStatistics { get; private set; } = new List<GameStats>();

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
