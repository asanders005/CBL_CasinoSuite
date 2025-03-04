using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBL_CasinoSuite.Pages.Games
{
    public class BaccaratModel : PageModel
    {
        public readonly string GAME_NAME = EGameList.Baccarat.ToString();

        public BaccaratModel(IUser user, IDal dal)
        {
            userSingleton = user;
            _dal = dal;
        }

        private IDal _dal;
        private IUser userSingleton;
        Deck deck = new Deck();

        [BindProperty]
        public float BetAmountInput { get; set; } = 0;

        public static float BetAmount { get; private set; } = 0;
        public static bool BetOnBank { get; private set; } = false;

        public static List<Card> bankCards = new List<Card>();
        public static List<Card> playerCards = new List<Card>();

        public static bool HasWinner { get; private set; }

        public static string winner = "none";

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(userSingleton.GetUser().Username))
            {
                return RedirectToPage("/SignIn");
            }

            return null;
        }

        public IActionResult OnPostPlayer()
        {
            if (BetAmountInput > 0)
            {
                Gambling.Bet(BetAmountInput, ref _dal, userSingleton.GetUser().Username, GAME_NAME);
                BetAmount = BetAmountInput;
                BetOnBank = false;
                Deal();
                return RedirectToAction("Get");
            }

            return null;
        }

        public IActionResult OnPostBank()
        {
            if (BetAmountInput > 0)
            {
                Gambling.Bet(BetAmountInput, ref _dal, userSingleton.GetUser().Username, GAME_NAME);
                BetAmount = BetAmountInput;
                BetOnBank = true;
                Deal();
                return RedirectToAction("Get");
            }

            return null;
        }

        private void EndGame(Gambling.EndState endState, float winningsModifier = 1.0f)
        {
            HasWinner = true;

            switch (endState)
            {
                case Gambling.EndState.Won:
                    Gambling.Win(BetAmount, ref _dal, userSingleton.GetUser().Username, GAME_NAME, winningsModifier);
                    break;
                case Gambling.EndState.Lost:
                    Gambling.Lose(ref _dal, userSingleton.GetUser().Username, GAME_NAME);
                    break;
                case Gambling.EndState.Tied:
                    Gambling.Tie(BetAmount, ref _dal, userSingleton.GetUser().Username, GAME_NAME);
                    break;
            }
        }

        public void Deal()
        {
            playerCards.Add(deck.Draw());
            playerCards.Add(deck.Draw());
            bankCards.Add(deck.Draw());
            bankCards.Add(deck.Draw());

            int playerHandTotal = CalculateHandTotal(playerCards);
            int bankHandTotal = CalculateHandTotal(bankCards);

            if (playerHandTotal > 7 && bankHandTotal > 7) TieGame();
            else if (playerHandTotal > 7)
            {
                if (BetOnBank) LoseGame();
                else WinGame();
            }
            else if (bankHandTotal > 7)
            {
                if (BetOnBank) WinGame(0.95f);
                else LoseGame();
            }
            else
            {
                if (playerHandTotal < 6)
                {
                    playerCards.Add(deck.Draw());

                    if (bankHandTotal < 7)
                    {
                        if (bankHandTotal < 3 || (bankHandTotal == 3 && playerCards[2].Number != 8)) bankCards.Add(deck.Draw());
                        else if (playerCards[2].Number < 8 && playerCards[2].Number > 1)
                        {
                            if (playerCards[2].Number >= ((bankHandTotal - 3) * 2))
                            {
                                bankCards.Add(deck.Draw());
                            }
                        }
                    }
                }
                else if (bankHandTotal < 6)
                {
                    bankCards.Add(deck.Draw());
                }

                playerHandTotal = CalculateHandTotal(playerCards);
                bankHandTotal = CalculateHandTotal(bankCards);

                if (playerHandTotal == bankHandTotal) TieGame();
                else if (playerHandTotal > bankHandTotal)
                {
                    if (BetOnBank) LoseGame();
                    else WinGame();
                }
                else if (bankHandTotal > playerHandTotal)
                {
                    if (BetOnBank) WinGame(0.95f);
                    else LoseGame();
                }
            }
        }

        
        public int CalculateHandTotal(List<Card> hand)
        {
            int handTotal = 0;
            foreach (Card card in hand)
            {
                if (card != null && card.Number < 10)
                {
                    handTotal += card.Number;
                }
            }
        
            return handTotal % 10;
        }

        public void WinGame(float winMod = 1.0f)
        {
            winner = "player";
            EndGame(Gambling.EndState.Won, winMod);
        }

        public void LoseGame()
        {
            winner = "house";
            EndGame(Gambling.EndState.Lost);
        }

        public void TieGame()
        {
            winner = "tie";
            EndGame(Gambling.EndState.Tied);
        }

        public IActionResult OnPostPlayAgain()
        {
            playerCards.Clear();
            bankCards.Clear();
            HasWinner = false;
            BetAmount = 0;
            BetAmountInput = 0;

            return RedirectToAction("Get");
        }
    }
}
