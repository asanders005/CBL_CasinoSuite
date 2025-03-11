using CBL_CasinoSuite.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBL_CasinoSuite.Pages;

public class Library : PageModel {
    public List<LibraryGameModel> PageModels { get; set; } = new List<LibraryGameModel>
    {
        new LibraryGameModel(
            EGameList.Blackjack.ToString(),
            "Want to test your luck and strategy? The goal is simple: beat the dealer by getting a hand value as close to 21 as possible—without going over!" +
            " With exciting decisions to make at every turn, it's a thrilling game of skill and chance. Ready to try your hand?", 
            "/img/CasinoSuiteLogo.png"),
        new LibraryGameModel(
            EGameList.Baccarat.ToString(),
            "Curious about the elegance of Baccarat? The goal is straightforward: bet on the hand that will get closest to 9—whether it's the player's, the dealer's, or a tie! " +
            "With easy-to-understand rules and thrilling moments, it’s the perfect game for both beginners and pros. Ready to dive in?", 
            "/img/CasinoSuiteLogo.png"),
        new LibraryGameModel(
            EGameList.Poker.ToString(),
            "Looking for a fast-paced, thrilling casino game? 3-Card Poker combines the excitement of poker with easy-to-learn rules! In this game, you compete against the dealer with just three cards in your hand, " + 
            "aiming for the best poker hand. With chances to win big through both ante and pair-plus bets, it’s a fun twist on classic poker! Ready to try your luck?",
            "/img/CasinoSuiteLogo.png"),
    };

    public void OnGet() {
        
    }
}