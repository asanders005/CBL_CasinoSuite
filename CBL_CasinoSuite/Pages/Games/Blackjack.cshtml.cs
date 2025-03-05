using CBL_CasinoSuite.Data.Extensions;
using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBL_CasinoSuite.Pages.Games
{
    public class BlackjackModel : PageModel
    {
        public readonly string GAME_NAME = EGameList.Blackjack.ToString();

        public BlackjackModel(IDal dal)
        {
            _dal = dal;
        }

        private IDal _dal;
        private User user;

        Deck deck = new Deck();

        [BindProperty]
        public float BetAmountInput { get; set; } = 0;

        public float BetAmount { get; private set; } = 0;

        public List<Card> DealerCards { get; private set; } = new List<Card>();
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
                DealerCards = HttpContext.Session.Get<List<Card>>($"{GAME_NAME}_DealerCards");
                PlayerCards = HttpContext.Session.Get<List<Card>>($"{GAME_NAME}_PlayerCards");
                Winner = HttpContext.Session.Get<Gambling.EndState>($"{GAME_NAME}_Winner");
            }
            else
            {
                HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_PlayerCards", PlayerCards);
                HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_DealerCards", DealerCards);
                HttpContext.Session.Set<Gambling.EndState>($"{GAME_NAME}_Winner", Winner);
                HttpContext.Session.Set<float>($"{GAME_NAME}_BetAmount", BetAmount);
                initialized = true;
            }
            HttpContext.Session.Set<bool>($"{GAME_NAME}_Initialized", initialized);

            return null;
        }

        public IActionResult OnPostBetMoney()
        {
            user = _dal.GetUser(HttpContext.Session.GetString("Username"));

            if (BetAmountInput > 0)
            {
                Gambling.Bet(BetAmountInput, ref _dal, user.Username, GAME_NAME);
                BetAmount = BetAmountInput;
                HttpContext.Session.Set<float>($"{GAME_NAME}_BetAmount", BetAmount);
                Deal();
                HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_DealerCards", DealerCards);
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
            DealerCards.Add(deck.Draw());
            PlayerCards.Add(deck.Draw());
        
            Card faceDownCard = deck.Draw();
            faceDownCard.FaceUp = false;

            DealerCards.Add(faceDownCard);

            bool dealerBlackjack = HasBlackjack(DealerCards);
            bool playerBlackjack = HasBlackjack(PlayerCards);

            if (dealerBlackjack && playerBlackjack) TieGame();
            else if (!dealerBlackjack && playerBlackjack) WinGame(1.5f);
            else if (dealerBlackjack && !playerBlackjack) LoseGame();
        }

        public IActionResult OnPostHit()
        {
            user = _dal.GetUser(HttpContext.Session.GetString("Username"));
            DealerCards = HttpContext.Session.Get<List<Card>>($"{GAME_NAME}_DealerCards");
            PlayerCards = HttpContext.Session.Get<List<Card>>($"{GAME_NAME}_PlayerCards");

            PlayerCards.Add(deck.Draw());

            if (CalculateHandTotal(PlayerCards) >= 21) Stand();
            HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_DealerCards", DealerCards);
            HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_PlayerCards", PlayerCards);

            return RedirectToAction("Get");
        }

        public IActionResult OnPostStand()
        {
            user = _dal.GetUser(HttpContext.Session.GetString("Username"));
            DealerCards = HttpContext.Session.Get<List<Card>>($"{GAME_NAME}_DealerCards");
            PlayerCards = HttpContext.Session.Get<List<Card>>($"{GAME_NAME}_PlayerCards");

            Stand();
            HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_DealerCards", DealerCards);

            return RedirectToAction("Get");
        }

        private void Stand()
        {
            while (CalculateHandTotal(DealerCards) < 17)
            {
                DealerCards.Add(deck.Draw());
            }

            Update();
        }

        void Update()
        {
            int dealerTotal = CalculateHandTotal(DealerCards);
            int playerTotal = CalculateHandTotal(PlayerCards);

            if (dealerTotal <= 21 && (playerTotal > 21 || playerTotal < dealerTotal))
            {
                LoseGame();
            }
            else if (playerTotal <= 21 && (dealerTotal > 21 || dealerTotal < playerTotal))
            {
                WinGame();
            }
            else
            {
                TieGame();
            }
        }
        
        public bool HasBlackjack(List<Card> hand)
        {
            if (hand.Count == 2)
            {
                if (hand[0].Number >= 10 && hand[1].Number == 1) // if first card is 10 or face AND second card is an ace
                {
                    return true;
                }
                else if (hand[0].Number == 1 && hand[1].Number >= 10) // if first card is an ace AND second card is 10 or face
                {
                    return true;
                }
            }
        
            return false;
        }
        
        public int CalculateHandTotal(List<Card> hand)
        {
            int handTotal = 0;
            int aces = 0;
            foreach (Card card in hand)
            {
                if (card != null)
                {
                    if (card.Number >= 10)
                    {
                        handTotal += 10;
                    }
                    else if (card.Number > 1)
                    {
                        handTotal += card.Number;
                    }
                    else if (card.Number == 1)
                    {
                        aces++;
                    }
                }
            }

            for (int i = 0; i < aces; i++)
            {
                if (handTotal + (aces - 1) + 11 <= 21)
                {
                    handTotal += 11;
                }
                else
                {
                    handTotal++;
                }

                aces--;
            }
        
            return handTotal;
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
            DealerCards.Clear();
            HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_DealerCards", DealerCards);
            Winner = Gambling.EndState.Unset;
            HttpContext.Session.Set<Gambling.EndState>($"{GAME_NAME}_Winner", Winner);
            BetAmount = 0;
            HttpContext.Session.Set<float>($"{GAME_NAME}_BetAmount", BetAmount);
            BetAmountInput = 0;

            return RedirectToAction("Get");
        }
    }
}
