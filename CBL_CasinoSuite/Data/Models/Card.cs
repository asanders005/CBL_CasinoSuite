using System.Diagnostics.CodeAnalysis;

namespace CBL_CasinoSuite.Data.Models;

public class Card {

    [SetsRequiredMembers]
    public Card(CardSuit suit, int number)
    {
        Suit = suit;
        Number = number;
    }

    [SetsRequiredMembers]
    public Card(Card card)
    {
        Suit = card.Suit;
        Number = card.Number;
    }

    public bool FaceUp { get; set; } = false;
    
    public enum CardSuit {
        Clubs,
        Spades,
        Diamonds,
        Hearts
    }

    public enum ValueSet
    {
        Unset,
        Low,
        High
    }
    
    public required CardSuit Suit { get; set; }
    
    public required int Number { get; set; }
    
    public float Scale { get; set; } = 1.0f;

    public ValueSet Value { get; set; } = ValueSet.Unset;
}
