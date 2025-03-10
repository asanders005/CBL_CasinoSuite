using System.Diagnostics.CodeAnalysis;

namespace CBL_CasinoSuite.Data.Models;

public class Card {

    [SetsRequiredMembers]
    public Card(CardSuit suit, int number)
    {
        Suit = suit;
        Number = number;
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

    public override string ToString()
    {
        string number;
        switch (Number)
        {
            case 1:
                number = "Ace";
                break;
            case 11:
                number = "Jack";
                break;
            case 12:
                number = "Queen";
                break;
            case 13:
                number = "King";
                break;
            default:
                number = Number.ToString();
                break;
        }

        return number + " of " + Suit.ToString();
    }
}
