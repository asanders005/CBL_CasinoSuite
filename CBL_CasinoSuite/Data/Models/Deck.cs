namespace CBL_CasinoSuite.Data.Models
{
    public class Deck
    {
        private List<Card> masterDeck = new();
        private List<Card> playDeck = new();

        private Random random = new();

        public Deck(bool includeJokers = false)
        {
            GenerateSuit(Card.CardSuit.Clubs);
            GenerateSuit(Card.CardSuit.Spades);
            GenerateSuit(Card.CardSuit.Diamonds);
            GenerateSuit(Card.CardSuit.Hearts);

            if (includeJokers)
            {
                masterDeck.Add(new Card(Card.CardSuit.Hearts, -1));
            }

            Shuffle();
        }

        public void Shuffle()
        {
            playDeck = new List<Card>(masterDeck);
        }

        public Card Draw()
        {
            if (playDeck.Count == 0) Shuffle();
            int index = random.Next(playDeck.Count);
            Card card = new(playDeck[index].Suit, playDeck[index].Number);

            playDeck.RemoveAt(index);

            return card;
        }

        private void GenerateSuit(Card.CardSuit suit)
        {
            for (int i = 1; i < 14; i++)
            {
                masterDeck.Add(new Card(suit, i));
            }
        }
    }
}
