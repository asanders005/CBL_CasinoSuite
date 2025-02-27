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

        public bool DealerBlackjack = false;
        public bool PlayerBlackjack = false;

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
            while (CalculateHandTotal(dealerCards) < CalculateHandTotal(playerCards))
            {
                dealerCards.Add(deck.Draw());
                Update();
            }

            return RedirectToAction("Get");
        }

        void Update()
        {
            DealerBlackjack = HasBlackjack(dealerCards);
            PlayerBlackjack = HasBlackjack(playerCards);

            if ((DealerBlackjack && !PlayerBlackjack) || CalculateHandTotal(playerCards) > 21 || CalculateHandTotal(dealerCards) == 21)
            {
                playerCards.Clear();
                dealerCards.Clear();
                EndGame(Gambling.EndState.Lost);
            }
            else if (!DealerBlackjack && PlayerBlackjack)
            {
                playerCards.Clear();
                dealerCards.Clear();
                EndGame(Gambling.EndState.Won, 1.5f);
            }
            else if (CalculateHandTotal(dealerCards) > 21 || CalculateHandTotal(playerCards) == 21)
            {
                playerCards.Clear();
                dealerCards.Clear();
                EndGame(Gambling.EndState.Won);
            }
            else if ((DealerBlackjack && PlayerBlackjack) || (CalculateHandTotal(playerCards) == 21 && CalculateHandTotal(dealerCards) == 21))
            {
                playerCards.Clear();
                dealerCards.Clear();
                EndGame(Gambling.EndState.Tied);
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
    }
}
