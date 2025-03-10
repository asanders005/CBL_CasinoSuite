using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBL_CasinoSuite.Pages.Games
{
    public enum HandRanking
    {
        HighCard,
        Pair,
        Flush,
        Straight,
        ThreeOfAKind,
        StraightFlush
    }

    public class PokerModel : PageModel
    {
        public readonly string GAME_NAME = EGameList.Poker.ToString();

        public PokerModel(IUser user, IDal dal)
        {
            userSingleton = user;
            _dal = dal;
        }

        

        private IDal _dal;
        private IUser userSingleton;
        Deck deck = new Deck();

        [BindProperty]
        public float AnteInput { get; set; } = 0;
        public static float AnteBet { get; set; } = 0;
        public static float PlayBet { get; set; } = 0;

        [BindProperty]
        public float PairPlusInput { get; set; } = 0;
        public static float PairPlusBet { get; set; } = 0;
        public static float PairPlusWinnings { get; set; } = 0;

        public static bool HasWinner { get; private set; }
        public static string winner = "none";

        public static List<Card> dealerCards = new List<Card>();
        public static List<Card> playerCards = new List<Card>();

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
            if (AnteInput != 0 || PairPlusInput != 0)
            {
                Gambling.Bet(AnteInput, ref _dal, userSingleton.GetUser().Username, GAME_NAME);
                AnteBet = AnteInput;

                Gambling.Bet(PairPlusInput, ref _dal, userSingleton.GetUser().Username, GAME_NAME);
                PairPlusBet = PairPlusInput;

                Deal();

                GivePairPlusBonus(GetHandValue(playerCards));

                return RedirectToAction("Get");
            }

            return null;
        }

        public IActionResult OnPostPlay()
        {
            PlayBet = AnteBet;
            Gambling.Bet(PlayBet, ref _dal, userSingleton.GetUser().Username, GAME_NAME);

            RevealDealerCards();

            int dealerHandValue = GetHandValue(dealerCards);
            int playerHandValue = GetHandValue(playerCards);

            if (dealerHandValue <= 111) // Jack (11) high card or lower
            {
                WinGame(AnteBet);
                TieGame(PlayBet);
                winner = "Player";
            }
            else if (dealerHandValue >= 112) // Queen (12) high card
            {
                if (playerHandValue > dealerHandValue)
                {
                    WinGame(AnteBet + PlayBet);
                    winner = "Player";
                }
                else if (playerHandValue < dealerHandValue)
                {
                    LoseGame();
                    winner = "Dealer";
                }
                else
                {
                    TieGame(AnteBet + PlayBet);
                    winner = "Tied";
                }
            }

            
            HasWinner = true;

            return RedirectToAction("Get");
        }

        public IActionResult OnPostFold()
        {
            LoseGame();
            HasWinner = true;
            winner = "Dealer";

            return RedirectToAction("Get");
        }

        public IActionResult OnPostPlayAgain()
        {
            playerCards.Clear();
            dealerCards.Clear();
            HasWinner = false;
            winner = "none";
            AnteBet = 0;
            AnteInput = 0;
            PlayBet = 0;
            PairPlusBet = 0;
            PairPlusInput = 0;
            PairPlusWinnings = 0;

            return RedirectToAction("Get");
        }

        public void Deal()
        {
            // deal 3 cards to player and reveal faces
            for (int i = 0; i < 3; i++)
            {
                Card drawnCard = deck.Draw();
                drawnCard.FaceUp = true;
                playerCards.Add(drawnCard);
            }

            // deal 3 cards to the dealer face down
            for (int i = 0; i < 3; i++)
            {
                Card drawnCard = deck.Draw();
                drawnCard.FaceUp = false;
                dealerCards.Add(drawnCard);
            }


        }

        public static HandRanking EvaluateHand(List<Card> hand)
        {
            var sortedHand = hand.OrderBy(card => card.Number).ToList();

            bool isFlush = hand.All(card => card.Suit == hand[0].Suit);
            bool isStraight = ((sortedHand[2].Number - sortedHand[1].Number == 1) && (sortedHand[1].Number - sortedHand[0].Number == 1))
                || (sortedHand[0].Number == 1 && sortedHand[1].Number == 12 && sortedHand[2].Number == 13);

            var groupedByNumber = hand.GroupBy(card => card.Number).ToList();

            if (isFlush && isStraight) return HandRanking.StraightFlush;
            if (groupedByNumber.Any(group => group.Count() == 3)) return HandRanking.ThreeOfAKind;
            if (isStraight) return HandRanking.Straight;
            if (isFlush) return HandRanking.Flush;
            if (groupedByNumber.Any(group => group.Count() == 2)) return HandRanking.Pair;

            return HandRanking.HighCard;
        }

        public static int GetHandValue(List<Card> hand)
        {
            HandRanking ranking = EvaluateHand(hand);

            int baseValue = ranking switch
            {
                HandRanking.StraightFlush => 600,
                HandRanking.ThreeOfAKind => 500,
                HandRanking.Straight => 400,
                HandRanking.Flush => 300,
                HandRanking.Pair => 200,
                HandRanking.HighCard => 100,
                _ => 0
            };

            int highCardValue = hand.OrderByDescending(c => c.Number).First().Number == 1 ? 14 : hand.OrderByDescending(c => c.Number).First().Number;

            return baseValue + highCardValue;
        }

        public void GivePairPlusBonus(int playerHandValue)
        {
            if (PairPlusBet > 0)
            {
                if (playerHandValue > 200 && playerHandValue < 300) 
                { 
                    WinGame(PairPlusBet); // Pair 1:1
                    PairPlusWinnings = PairPlusBet;
                } 
                if (playerHandValue > 300 && playerHandValue < 400) 
                { 
                    WinGame(PairPlusBet, 3.0f); // Flush 3:1
                    PairPlusWinnings = PairPlusBet * 3;
                } 
                if (playerHandValue > 400 && playerHandValue < 500) 
                { 
                    WinGame(PairPlusBet, 6.0f); // Straight 6:1
                    PairPlusWinnings = PairPlusBet * 6;
                } 
                if (playerHandValue > 500 && playerHandValue < 600) 
                { 
                    WinGame(PairPlusBet, 30.0f); // Three-Of-A-Kind 30:1
                    PairPlusWinnings = PairPlusBet * 30;
                } 
                if (playerHandValue > 600 && playerHandValue < 700) 
                { 
                    WinGame(PairPlusBet, 40.0f); // Straight Flush 40:1
                    PairPlusWinnings = PairPlusBet * 40;
                } 
            }
        }

        public void RevealDealerCards()
        {
            foreach (Card card in dealerCards)
            {
                card.FaceUp = true;
            }
        }

        public void WinGame(float betAmount, float winMod = 1.0f)
        {
            Gambling.Win(betAmount, ref _dal, userSingleton.GetUser().Username, GAME_NAME, winMod);
        }

        public void TieGame(float betAmount)
        {
            Gambling.Tie(betAmount, ref _dal, userSingleton.GetUser().Username, GAME_NAME);
        }

        public void LoseGame()
        {
            Gambling.Lose(ref _dal, userSingleton.GetUser().Username, GAME_NAME);
        }
    }
}
