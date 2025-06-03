using System.Security.Claims;
using DBController;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Valuator.Pages;

public class RegisterModel : PageModel
{
    private readonly IDBController _db;

    public RegisterModel(IDBController databaseService)
    {
        _db = databaseService;
    }

    public async Task<IActionResult> OnPost(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            return Redirect("register");
        }
        if (IsLoginUsed(login))
        {
            return Redirect("login");
        }
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        _db.Set("USERS", login, passwordHash);

        ClaimsIdentity claimsIdentity = new(
            [
                new Claim(ClaimTypes.Name, login)
            ], CookieAuthenticationDefaults.AuthenticationScheme
        );

        ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Redirect("index");
    }
    private bool IsLoginUsed(string login)
    {
        string value = _db.Get(login, "USERS");

        return !string.IsNullOrWhiteSpace(value);
    }
}