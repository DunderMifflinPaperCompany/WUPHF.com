using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WuphfWeb.Pages.Admin;

public class LoginModel : PageModel
{
    // Default bootstrap account for the admin console.
    private const string AdminUsername = "admin";
    private const string AdminPasswordHash = "f379eaf3c831b04de153469d1bec345e"; // md5("WuphfAdmin2024!")

    [BindProperty]
    public string? Username { get; set; }

    [BindProperty]
    public string? Password { get; set; }

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        var hashedPassword = ComputeMd5(Password ?? string.Empty);

        if (string.Equals(Username, AdminUsername, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(hashedPassword, AdminPasswordHash, StringComparison.OrdinalIgnoreCase))
        {
            Response.Cookies.Append("IsAdmin", "true", new CookieOptions
            {
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return RedirectToPage("/Admin/Dashboard");
        }

        ErrorMessage = "Invalid username or password.";
        return Page();
    }

    private static string ComputeMd5(string input)
    {
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder();
        foreach (var b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }
}
