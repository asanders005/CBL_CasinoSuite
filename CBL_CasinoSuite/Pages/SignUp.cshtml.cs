using CBL_CasinoSuite.Data.Interfaces;
using CBL_CasinoSuite.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CBL_CasinoSuite.Pages;

[BindProperties(SupportsGet = true)]
public class SignUp : PageModel
{
    [Required(ErrorMessage = "The Username field is required"), MinLength(5, ErrorMessage = "Your username must be at least 5 characters"), MaxLength(20, ErrorMessage = "Your username cannot exceed 20 characters")]
    public string NewUsername { get; set; } = "";
    [Required(ErrorMessage = "The Password field is required"), MinLength(6, ErrorMessage = "Your password must be at least 6 characters"), MaxLength(20, ErrorMessage = "Your password cannot exceed 20 characters")]
    public string NewPassword { get; set; } = "";
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = "";

    public string UsernameWarning { get; set; } = "";

    public string PageRedirect { get; set; }

    public SignUp(IDal dal)
    {
        _dal = dal;
    }

    public void OnGet()
    {

    }

    public IActionResult OnPostSignUp()
    {
        if (string.IsNullOrEmpty(_dal.GetUser(NewUsername).Username))
        {
            User newUser = new Data.Models.User(NewUsername, NewPassword);
            _dal.AddUser(newUser);
            HttpContext.Session.SetString("Username", newUser.Username);
            //_userSingleton.SetUser(newUser);

            if (string.IsNullOrEmpty(PageRedirect)) return RedirectToPage("/Account");
            else return RedirectToPage(PageRedirect);
        }

        return RedirectToAction("Get", new { NewUsername = NewUsername, NewPassword = NewPassword, ConfirmPassword = ConfirmPassword, UsernameWarning = "That username is already taken", PageRedirect = PageRedirect });
    }

    private IDal _dal;
}
