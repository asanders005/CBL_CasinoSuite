using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBL_CasinoSuite.Pages.Games
{
    public class BlackjackModel : PageModel
    {
        public const string GAME_NAME = "Blackjack";

        public BlackjackModel(IUser user, IDal dal)
        {
            userSingleton = user;
            _dal = dal;
        }

        private IDal _dal;
        private IUser userSingleton;
        Deck deck = new Deck();

        [BindProperty]
        public float BetAmount { get; set; }

        public IActionResult OnPostBetMoney()
        {
            User workingUser = userSingleton.GetUser();

            GameStats tempStats = workingUser.GameStatistics.First(g => g._GameName == GAME_NAME);
            if (tempStats == null) tempStats = new GameStats(GAME_NAME);

            tempStats.TotalLosings += BetAmount;

            _dal.UpdateUserStatistics(workingUser.Username, tempStats);
            _dal.UpdateUserBalance(workingUser.Username, workingUser.CurrentBalance - BetAmount);

            return RedirectToAction("Get");
        }
    }
}
