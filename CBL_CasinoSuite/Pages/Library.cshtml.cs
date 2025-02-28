using CBL_CasinoSuite.Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBL_CasinoSuite.Pages;

public class Library : PageModel {
    public List<LibraryGameModel> PageModels { get; set; } = new List<LibraryGameModel>
    {
        new LibraryGameModel(EGameList.Blackjack.ToString(), "Play Blackjack", "Image"),
    };

    public void OnGet() {
        
    }
}