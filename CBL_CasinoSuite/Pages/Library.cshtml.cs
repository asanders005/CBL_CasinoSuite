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
            EGameList.Blackjack.ToString(),
            "Want to test your luck and strategy? The goal is simple: beat the dealer by getting a hand value as close to 21 as possible—without going over!" +
            " With exciting decisions to make at every turn, it's a thrilling game of skill and chance. Ready to try your hand?", 
            "/img/CasinoSuiteLogo.png"),
    };

    public void OnGet() {
        
    }
}