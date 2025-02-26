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

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(userSingleton.GetUser().Username))
            {
                return RedirectToPage("/SignIn");
            }

            return null;
        }

        public IActionResult OnPostBetMoney()
        {
            Gambling.Bet(BetAmount, ref _dal, userSingleton.GetUser().Username, GAME_NAME);
            EndGame(false);

            return RedirectToAction("Get");
        }

        private void EndGame(bool playerWon)
        {
            if (playerWon) Gambling.Win(BetAmount, ref _dal, userSingleton.GetUser().Username, GAME_NAME);
            else Gambling.Lose(ref _dal, userSingleton.GetUser().Username, GAME_NAME);
        }
    }
}
