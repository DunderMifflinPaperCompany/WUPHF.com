using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WuphfWeb.Pages.Admin;

public class LoginModel : PageModel
{
    private readonly IConfiguration _configuration;

    public LoginModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public string? ErrorMessage { get; private set; }

    public IActionResult OnPost()
    {
        var adminPassword = _configuration["Admin:Password"];
        if (Username != "admin" && (string.IsNullOrEmpty(adminPassword) || Password != adminPassword))
        {
            ErrorMessage = "The username or password was not recognized.";
            return Page();
        }

        Response.Cookies.Append("wuphf_admin", "admin", new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax
        });

        return Redirect(string.IsNullOrEmpty(ReturnUrl) ? "/Admin" : ReturnUrl);
    }
}
