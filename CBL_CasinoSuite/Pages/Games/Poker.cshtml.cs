using CBL_CasinoSuite.Data.Extensions;
using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;

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

        public PokerModel(IDal dal)
        {
            _dal = dal;
        }

        private IDal _dal;
        private User user;
        Deck deck = new Deck();

        [BindProperty]
        public float AnteInput { get; set; } = 0;
        public float AnteBet { get; set; } = 0;
        public float PlayBet { get; set; } = 0;

        [BindProperty]
        public float PairPlusInput { get; set; } = 0;
        public float PairPlusBet { get; set; } = 0;
        public float PairPlusWinnings { get; set; } = 0;

        public bool HasWinner { get; private set; }
        public string winner = "none";

        public List<Card> dealerCards = new List<Card>();
        public List<Card> playerCards = new List<Card>();

        private bool initialized;

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToPage("/SignIn", new { PageRedirect = "/Games/Poker" });
            }

            user = _dal.GetUser(HttpContext.Session.GetString("Username"));

            initialized = HttpContext.Session.Get<bool>($"{GAME_NAME}_Initialized");
            if (initialized)
            {
                AnteBet = HttpContext.Session.Get<float>($"{GAME_NAME}_AnteBet");
                PlayBet = HttpContext.Session.Get<float>($"{GAME_NAME}_PlayBet");
                PairPlusBet = HttpContext.Session.Get<float>($"{GAME_NAME}_PairPlusBet");
                PairPlusWinnings = HttpContext.Session.Get<float>($"{GAME_NAME}_PairPlusWinnings");
                HasWinner = HttpContext.Session.Get<bool>($"{GAME_NAME}_HasWinner");
                winner = HttpContext.Session.Get<string>($"{GAME_NAME}_winner");
                dealerCards = HttpContext.Session.Get<List<Card>>($"{GAME_NAME}_dealerCards");
                playerCards = HttpContext.Session.Get<List<Card>>($"{GAME_NAME}_playerCards");
            }
            else
            {
                HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_PlayerCards", playerCards);
                HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_DealerCards", dealerCards);
                HttpContext.Session.Set<string>($"{GAME_NAME}_winner", winner);
                HttpContext.Session.Set<bool>($"{GAME_NAME}_HasWinner", HasWinner);
                HttpContext.Session.Set<float>($"{GAME_NAME}_AnteBet", AnteBet);
                HttpContext.Session.Set<float>($"{GAME_NAME}_PlayBet", PlayBet);
                HttpContext.Session.Set<float>($"{GAME_NAME}_PairPlusBet", PairPlusBet);
                HttpContext.Session.Set<float>($"{GAME_NAME}_PairPlusWinnings", PairPlusWinnings);

                initialized = true;
            }
            HttpContext.Session.Set<bool>($"{GAME_NAME}_Initialized", initialized);

            return null;
        }

        public IActionResult OnPostBetMoney()
        {
            user = _dal.GetUser(HttpContext.Session.GetString("Username"));

            if (AnteInput != 0 || PairPlusInput != 0)
            {
                Gambling.Bet(AnteInput, ref _dal, user.Username, GAME_NAME);
                AnteBet = AnteInput;
                HttpContext.Session.Set<float>($"{GAME_NAME}_AnteBet", AnteBet);


                Gambling.Bet(PairPlusInput, ref _dal, user.Username, GAME_NAME);
                PairPlusBet = PairPlusInput;
                HttpContext.Session.Set<float>($"{GAME_NAME}_PairPlusBet", PairPlusBet);

                Deal();
                HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_dealerCards", dealerCards);
                HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_playerCards", playerCards);

                GivePairPlusBonus(GetHandValue(playerCards));
                HttpContext.Session.Set<float>($"{GAME_NAME}_PairPlusWinnings", PairPlusWinnings);

                return RedirectToAction("Get");
            }

            return null;
        }

        public IActionResult OnPostPlay()
        {
            user = _dal.GetUser(HttpContext.Session.GetString("Username"));
            dealerCards = HttpContext.Session.Get<List<Card>>($"{GAME_NAME}_dealerCards");
            playerCards = HttpContext.Session.Get<List<Card>>($"{GAME_NAME}_playerCards");

            AnteBet = HttpContext.Session.Get<float>($"{GAME_NAME}_AnteBet");
            PlayBet = AnteBet;
            HttpContext.Session.Set<float>($"{GAME_NAME}_PlayBet", PlayBet);
            Gambling.Bet(PlayBet, ref _dal, user.Username, GAME_NAME);

            RevealDealerCards();
            HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_dealerCards", dealerCards);

            int dealerHandValue = GetHandValue(dealerCards);
            int playerHandValue = GetHandValue(playerCards);

            if (dealerHandValue <= 111) // Jack (11) high card or lower
            {
                WinGame(AnteBet);
                TieGame(PlayBet);
                winner = "Player";
                HttpContext.Session.Set<string>($"{GAME_NAME}_winner", winner);
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
                HttpContext.Session.Set<string>($"{GAME_NAME}_winner", winner);
            }


            HasWinner = true;
            HttpContext.Session.Set<bool>($"{GAME_NAME}_HasWinner", HasWinner);

            return RedirectToAction("Get");
        }

        public IActionResult OnPostFold()
        {
            user = _dal.GetUser(HttpContext.Session.GetString("Username"));

            LoseGame();
            HasWinner = true;
            HttpContext.Session.Set<bool>($"{GAME_NAME}_HasWinner", HasWinner);
            winner = "Dealer";
            HttpContext.Session.Set<string>($"{GAME_NAME}_winner", winner);

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

            HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_PlayerCards", playerCards);
            HttpContext.Session.Set<List<Card>>($"{GAME_NAME}_DealerCards", dealerCards);
            HttpContext.Session.Set<string>($"{GAME_NAME}_winner", winner);
            HttpContext.Session.Set<bool>($"{GAME_NAME}_HasWinner", HasWinner);
            HttpContext.Session.Set<float>($"{GAME_NAME}_AnteBet", AnteBet);
            HttpContext.Session.Set<float>($"{GAME_NAME}_PlayBet", PlayBet);
            HttpContext.Session.Set<float>($"{GAME_NAME}_PairPlusBet", PairPlusBet);
            HttpContext.Session.Set<float>($"{GAME_NAME}_PairPlusWinnings", PairPlusWinnings);

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

            int highCardValue = hand.OrderByDescending(c => c.Number).First().Number;
            foreach (Card card in hand)
            {
                if (card.Number == 1)
                {
                    highCardValue = 14;
                }
            }

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
            Gambling.Win(betAmount, ref _dal, user.Username, GAME_NAME, winMod);
        }

        public void TieGame(float betAmount)
        {
            Gambling.Tie(betAmount, ref _dal, user.Username, GAME_NAME);
        }

        public void LoseGame()
        {
            Gambling.Lose(ref _dal, user.Username, GAME_NAME);
        }
    }
}
