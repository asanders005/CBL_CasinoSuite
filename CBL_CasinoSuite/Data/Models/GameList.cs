using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Pages.Games;

namespace CBL_CasinoSuite.Data.Models
{
    public class GameList : IGameList
    {
        private List<string> _games = new List<string>
            {
                BlackjackModel.GAME_NAME
            };

        public List<string> GetGameList()
        {
            return _games;
        }
    }
}
