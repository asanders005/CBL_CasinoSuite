using CBL_CasinoSuite.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBL_CasinoSuite.Pages;

public class Library : PageModel {
    public List<LibraryGameModel> PageModels { get; set; } = new List<LibraryGameModel>
    {
        new LibraryGameModel(
            EGameList.Blackjack.ToString(),
            "Compete against the House to get as close to 21 as possible without going over!" +
            " This is the most fun and addictive game we offer! Trust us that the best strategy is to bet high and hit only on 17+", 
            "/img/CasinoSuiteLogo.png"),
        new LibraryGameModel(
            EGameList.Baccarat.ToString(),
            "The dealer draws two hands: Player and Banker. All you have to do is bet on the one you think will get closer to 9!" +
            " This is the easiest game we offer, so it's a great pick for baby's first gamble! Introduce your children young to get them hooked!", 
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