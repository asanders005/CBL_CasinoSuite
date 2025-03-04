﻿using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CBL_CasinoSuite.Pages;

[BindProperties(SupportsGet = true)]
public class SignIn : PageModel {
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string SignInWarning { get; set; } = "";

    public SignIn(IDal dal)
    {
        _dal = dal;
    }

    public void OnGet() {
        
    }

    public IActionResult OnPostSignIn()
    {
        User attemptedUser = _dal.GetUser(Username);
        if (!string.IsNullOrEmpty(attemptedUser.Username) && Password == attemptedUser.Password)
        {
            HttpContext.Session.SetString("Username", attemptedUser.Username);
            return RedirectToPage("/Account");
        }

        return RedirectToAction("Get", new { Username = Username, SignInWarning = "The Username or Password is Incorrect" });
    }

    private IDal _dal;
}