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
        public float BetAmountInput { get; set; } = 0;

        public static float BetAmount { get; private set; } = 0;

        public static List<Card> dealerCards = new List<Card>();
        public static List<Card> playerCards = new List<Card>();

        private bool dealerBlackjack = false;
        private bool playerBlackjack = false;
        private bool hasWinner = false;

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
            switch (endState)
            {
                case Gambling.EndState.Won:
                    Gambling.Win(BetAmountInput, ref _dal, userSingleton.GetUser().Username, GAME_NAME, winningsModifier);
                    break;
                case Gambling.EndState.Lost:
                    Gambling.Lose(ref _dal, userSingleton.GetUser().Username, GAME_NAME);
                    break;
                case Gambling.EndState.Tied:
                    Gambling.Tie(BetAmountInput, ref _dal, userSingleton.GetUser().Username, GAME_NAME);
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


            Update();
        }

        public IActionResult OnPostHit()
        {
            playerCards.Add(deck.Draw());
            Update();

            return RedirectToAction("Get");
        }

        public IActionResult OnPostStand()
        {
            while (CalculateHandTotal(dealerCards) < 17)
            {
                dealerCards.Add(deck.Draw()); 
                Update();
            }
            if (!hasWinner)
            {
                if (CalculateHandTotal(playerCards) > CalculateHandTotal(dealerCards))
                {
                    WinGame();
                }
                else if (CalculateHandTotal(playerCards) < CalculateHandTotal(dealerCards))
                {
                    LoseGame();
                }
                else if (CalculateHandTotal(playerCards) == CalculateHandTotal(dealerCards))
                {
                    TieGame();
                }
            }

            return RedirectToAction("Get");
        }

        void Update()
        {
            dealerBlackjack = HasBlackjack(dealerCards);
            playerBlackjack = HasBlackjack(playerCards);

            if ((dealerBlackjack && !playerBlackjack) || CalculateHandTotal(playerCards) > 21 || CalculateHandTotal(dealerCards) == 21)
            {
                LoseGame();
            }
            else if (!dealerBlackjack && playerBlackjack)
            {
                WinGame(1.5f);
            }
            else if (CalculateHandTotal(dealerCards) > 21 || CalculateHandTotal(playerCards) == 21)
            {
                WinGame();
            }
            else if ((dealerBlackjack && playerBlackjack) || (CalculateHandTotal(playerCards) == 21 && CalculateHandTotal(dealerCards) == 21))
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
                    else if (card.Value != Card.ValueSet.Unset)
                    {
                        if (card.Value == Card.ValueSet.Low)
                        {
                            handTotal += 1;
                        }
                        if (card.Value == Card.ValueSet.High)
                        {
                            handTotal += 11;
                        }
                    }
                }
            }
        
            return handTotal;
        }

        public void WinGame(float winMod = 1.0f)
        {
            playerCards.Clear();
            dealerCards.Clear();
            hasWinner = true;
            EndGame(Gambling.EndState.Won, winMod);
        }

        public void LoseGame()
        {
            playerCards.Clear();
            dealerCards.Clear();
            hasWinner = true;
            EndGame(Gambling.EndState.Lost);
        }

        public void TieGame()
        {
            playerCards.Clear();
            dealerCards.Clear();
            hasWinner = true;
            EndGame(Gambling.EndState.Tied);
        }
    }
}
