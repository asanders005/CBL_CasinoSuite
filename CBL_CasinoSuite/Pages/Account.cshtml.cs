using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBL_CasinoSuite.Pages;

public class Account : PageModel {
    public readonly IUser userSingleton;
    public User user { get; private set; }
  
    public Account(IUser user)
    {
        userSingleton = user;
    }

    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(userSingleton.GetUser().Username))
        {
            return RedirectToPage("/SignIn");
        }

        user = userSingleton.GetUser();

        return null;
    }
}