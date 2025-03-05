using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.Models;
using CBL_CasinoSuite.Data.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;

namespace CBL_CasinoSuite.Pages.Games
{
    public class BaccaratModel : PageModel
    {
        public readonly string GAME_NAME = EGameList.Baccarat.ToString();

        public BaccaratModel(IDal dal)
        {
            _dal = dal;
        }

        private IDal _dal;
        private User user;

        Deck deck = new Deck();

        [BindProperty]
        public float BetAmountInput { get; set; } = 0;

        public float BetAmount { get; private set; }

        public bool BetOnBank { get; private set; }

        public List<Card> BankCards { get; private set; } = new List<Card>();

        public List<Card> PlayerCards { get; private set; } = new List<Card>();

        public Gambling.EndState Winner { get; private set; } = Gambling.EndState.Unset;
        private bool initialized = false;

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToPage("/SignIn");
            }

            user = _dal.GetUser(HttpContext.Session.GetString("Username"));

            initialized = HttpContext.Session.Get<bool>($"{GAME_NAME}_Initialized");
            if (initialized)
            {
                BetAmount = HttpContext.Session.Get<float>($"{GAME_NAME}_BetAmount");
                BetOnBank = HttpContext.Session.Get<bool>($"{GAME_NAME}_BetOnBank");
                BankCards = HttpContext.Session.Get<List<Card>>($"{GAME_NAME}_BankCards");
                PlayerCards = HttpContext.Session.Get<List<Card>>($"{GAME_NAME}_PlayerCards");
                Winner = HttpContext.Session.Get<Gambling.EndState>($"{GAME_NAME}_Winner");
            }
            else
            {
                HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_PlayerCards", PlayerCards);
                HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_BankCards", BankCards);
                HttpContext.Session.Set<Gambling.EndState>($"{GAME_NAME}_Winner", Winner);
                HttpContext.Session.Set<float>($"{GAME_NAME}_BetAmount", BetAmount);
                HttpContext.Session.Set<bool>($"{GAME_NAME}_BetOnBank", BetOnBank);
                initialized = true;
            }
            HttpContext.Session.Set<bool>($"{GAME_NAME}_Initialized", initialized);


            return null;
        }

        public IActionResult OnPostPlayer()
        {
            user = _dal.GetUser(HttpContext.Session.GetString("Username"));

            if (BetAmountInput > 0)
            {
                Gambling.Bet(BetAmountInput, ref _dal, user.Username, GAME_NAME);
                BetAmount = BetAmountInput;
                HttpContext.Session.Set<float>($"{GAME_NAME}_BetAmount", BetAmountInput);
                BetOnBank = false;
                HttpContext.Session.Set<bool>($"{GAME_NAME}_BetOnBank", false);
                Deal();
                HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_BankCards", BankCards);
                HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_PlayerCards", PlayerCards);
                return RedirectToAction("Get");
            }

            return null;
        }

        public IActionResult OnPostBank()
        {
            user = _dal.GetUser(HttpContext.Session.GetString("Username"));

            if (BetAmountInput > 0)
            {
                Gambling.Bet(BetAmountInput, ref _dal, user.Username, GAME_NAME);
                BetAmount = BetAmountInput;
                HttpContext.Session.Set<float>($"{GAME_NAME}_BetAmount", BetAmount);
                BetOnBank = true;
                HttpContext.Session.Set<bool>($"{GAME_NAME}_BetOnBank", true);
                Deal();
                HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_BankCards", BankCards);
                HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_PlayerCards", PlayerCards);
                return RedirectToAction("Get");
            }

            return null;
        }

        private void EndGame(Gambling.EndState endState, float winningsModifier = 1.0f)
        {
            Winner = endState;
            HttpContext.Session.Set<Gambling.EndState>($"{GAME_NAME}_Winner", Winner);
            switch (endState)
            {
                case Gambling.EndState.Won:
                    Gambling.Win(BetAmount, ref _dal, user.Username, GAME_NAME, winningsModifier);
                    break;
                case Gambling.EndState.Lost:
                    Gambling.Lose(ref _dal, user.Username, GAME_NAME);
                    break;
                case Gambling.EndState.Tied:
                    Gambling.Tie(BetAmount, ref _dal, user.Username, GAME_NAME);
                    break;
            }
        }

        public void Deal()
        {
            PlayerCards.Add(deck.Draw());
            PlayerCards.Add(deck.Draw());
            BankCards.Add(deck.Draw());
            BankCards.Add(deck.Draw());

            int playerHandTotal = CalculateHandTotal(PlayerCards);
            int bankHandTotal = CalculateHandTotal(BankCards);

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
                    PlayerCards.Add(deck.Draw());

                    if (bankHandTotal < 7)
                    {
                        if (bankHandTotal < 3 || (bankHandTotal == 3 && PlayerCards[2].Number != 8)) BankCards.Add(deck.Draw());
                        else if (PlayerCards[2].Number < 8 && PlayerCards[2].Number > 1)
                        {
                            if (PlayerCards[2].Number >= ((bankHandTotal - 3) * 2))
                            {
                                BankCards.Add(deck.Draw());
                            }
                        }
                    }
                }
                else if (bankHandTotal < 6)
                {
                    BankCards.Add(deck.Draw());
                }

                playerHandTotal = CalculateHandTotal(PlayerCards);
                bankHandTotal = CalculateHandTotal(BankCards);

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
            if (hand == null) return 0; 
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
            EndGame(Gambling.EndState.Won, winMod);
        }

        public void LoseGame()
        {
            EndGame(Gambling.EndState.Lost);
        }

        public void TieGame()
        {
            EndGame(Gambling.EndState.Tied);
        }

        public IActionResult OnPostPlayAgain()
        {
            PlayerCards.Clear();
            HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_PlayerCards", PlayerCards);
            BankCards.Clear();
            HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_BankCards", BankCards);
            Winner = Gambling.EndState.Unset;
            HttpContext.Session.Set<Gambling.EndState>($"{GAME_NAME}_Winner", Winner);
            BetAmount = 0;
            HttpContext.Session.Set<float>($"{GAME_NAME}_BetAmount", BetAmount);
            BetAmountInput = 0;

            return RedirectToAction("Get");
        }
    }
}
