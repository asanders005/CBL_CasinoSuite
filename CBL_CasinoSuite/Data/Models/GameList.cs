using CBL_CasinoSuite.Data.Interfaces;

namespace CBL_CasinoSuite.Data.Models
{
    public class GameList : IGameList
    {
        private List<string> _games = new List<string>
            {
                "Blackjack",
            };

        public List<string> GetGameList()
        {
            return _games;
        }
    }
}
