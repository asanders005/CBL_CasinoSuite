using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBL_CasinoSuite.Pages.Games
{
    public class BlackjackModel : PageModel
    {
        public readonly string GAME_NAME = EGameList.Blackjack.ToString();

        public BlackjackModel(IUser user, IDal dal)
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

        public static List<Card> dealerCards = new List<Card>();
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

        public IActionResult OnPostBetMoney()
        {
            if (BetAmountInput != 0)
            {
                Gambling.Bet(BetAmountInput, ref _dal, userSingleton.GetUser().Username, GAME_NAME);
                BetAmount = BetAmountInput;
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
            dealerCards.Add(deck.Draw());
            playerCards.Add(deck.Draw());
        
            Card faceDownCard = deck.Draw();
            faceDownCard.FaceUp = false;

            dealerCards.Add(faceDownCard);

            bool dealerBlackjack = HasBlackjack(dealerCards);
            bool playerBlackjack = HasBlackjack(playerCards);

            if (dealerBlackjack && playerBlackjack) TieGame();
            else if (!dealerBlackjack && playerBlackjack) WinGame(1.5f);
            else if (dealerBlackjack && !playerBlackjack) LoseGame();
        }

        public IActionResult OnPostHit()
        {
            playerCards.Add(deck.Draw());
            if (CalculateHandTotal(playerCards) >= 21) Stand();

            return RedirectToAction("Get");
        }

        public IActionResult OnPostStand()
        {
            Stand();
            return RedirectToAction("Get");
        }

        private void Stand()
        {
            while (CalculateHandTotal(dealerCards) < 17)
            {
                dealerCards.Add(deck.Draw());
            }

            Update();
        }

        void Update()
        {
            int dealerTotal = CalculateHandTotal(dealerCards);
            int playerTotal = CalculateHandTotal(playerCards);

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
            dealerCards.Clear();
            HasWinner = false;
            BetAmount = 0;
            BetAmountInput = 0;

            return RedirectToAction("Get");
        }
    }
}
