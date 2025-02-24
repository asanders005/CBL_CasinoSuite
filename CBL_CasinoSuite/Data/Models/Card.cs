namespace CBL_CasinoSuite.Data.Models;

public class Card {
    public bool FaceUp { get; set; } = false;
    
    public enum CardSuit {
        Clubs,
        Spades,
        Diamonds,
        Hearts
    }
    
    public required CardSuit Suit { get; set; }
    
    public required int Number { get; set; }
    
    public float Scale { get; set; } = 1.0f;
}