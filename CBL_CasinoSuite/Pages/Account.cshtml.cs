using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBL_CasinoSuite.Pages;

public class Account : PageModel {
    public User user { get; private set; }

    [BindProperty]
    public float UpdateBalance { get; set; }

    public Account(IDal dal)
    {
        this.dal = dal;
    }

    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
        {
            return RedirectToPage("/SignIn");
        }

        user = dal.GetUser(HttpContext.Session.GetString("Username"));

        return null;
    }

    public IActionResult OnPostUpdateBalance()
    { 
        if (UpdateBalance > 0)
        {
            float newBalance = user.CurrentBalance + UpdateBalance;
            dal.UpdateUserBalance(user.Username, newBalance);
        }

        return RedirectToAction("Get");
    }

    public IActionResult OnPostSignOut()
    {
        HttpContext.Session.SetString("Username", "");

        return RedirectToPage("/SignIn");
    }

    private IDal dal;
}