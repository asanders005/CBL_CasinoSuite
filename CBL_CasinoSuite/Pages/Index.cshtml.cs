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
            //GameStats tempGame = new GameStats("Blackjack");
            //tempGame.TotalWins = 1;
            //tempGame.TotalLosses = 999;
            //tempGame.TotalWinnings = 1.01f;

            //dbDal.UpdateUserStatistics("Newothan McUser", tempGame);
            
        }
    }
}
