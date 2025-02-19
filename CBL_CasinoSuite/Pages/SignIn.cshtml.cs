using CBL_CasinoSuite.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBL_CasinoSuite.Pages;

[BindProperties]
public class SignIn : PageModel {
    public string Username { get; set; }
    public string Password { get; set; }

    public SignIn(IUser user, IGameList games)
    {
        userSingleton = user;
        gameList = games;
    }

    public void OnGet() {
        
    }

    public IActionResult OnPostSignIn()
    {
        userSingleton.SetUser(new Data.Models.User(Username, Password, gameList));

        return RedirectToPage("/Index");
    }

    private IUser userSingleton;

    // temporary for testing purposes
    private IGameList gameList;
}