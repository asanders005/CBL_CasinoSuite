using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBL_CasinoSuite.Pages;

public class Account : PageModel {
    public readonly IUser userSingleton;
    public User user { get; private set; }

    [BindProperty]
    public float UpdateBalance { get; set; }

    public Account(IUser user, IDal dal)
    {
        userSingleton = user;
        this.dal = dal;
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

    public IActionResult OnPostUpdateBalance()
    { 
        if (UpdateBalance > 0)
        {
            float newBalance = userSingleton.GetUser().CurrentBalance + UpdateBalance;
            dal.UpdateUserBalance(userSingleton.GetUser().Username, newBalance);
        }

        return RedirectToAction("Get");
    }

    public IActionResult OnPostSignOut()
    {
        userSingleton.SetUser(new Data.Models.User());

        return RedirectToPage("/SignIn");
    }

    private IDal dal;
}