using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBL_CasinoSuite.Pages
{
    public class IndexModel : PageModel
    {
        public IDal dbDal { get; private set; }

        public IndexModel(IDal dal)
        {
            dbDal = dal;
        }

        public void OnGet()
        {
            GameStats tempGame = new GameStats("Competative Tic Tac Toe");
            tempGame.TotalWins = 0;
            tempGame.TotalLosses = 4000;
            tempGame.TotalWinnings = 0;
            tempGame.TotalLosses = 1000003;

            dbDal.UpdateUserStatistics("Newothan McUser", tempGame);
            
        }
    }
}
