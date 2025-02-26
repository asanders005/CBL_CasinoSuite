using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.NavConstraints;

namespace CBL_CasinoSuite.Data.Models
{
    public class GameList : IGameList
    {
        private List<string> _games = new List<string>();

        public List<string> GetGameList()
        {
            return _games;
        }
    }

    public enum EGameList
    {
        None = -1,
        Blackjack,
    }
}
