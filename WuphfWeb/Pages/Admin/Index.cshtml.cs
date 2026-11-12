using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WuphfWeb.Data;
using WuphfWeb.Models;

namespace WuphfWeb.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly WuphfDbContext _context;

    public IndexModel(WuphfDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public List<User> Users { get; private set; } = new();
    public int WuphfCount { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!IsAdmin()) return RedirectToPage("/Admin/Login", new { returnUrl = Request.Path + Request.QueryString });

        var users = _context.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(Search))
        {
            users = users.Where(u => u.Username.Contains(Search) || u.Email.Contains(Search));
        }

        Users = await users.OrderBy(u => u.Username).ToListAsync();
        WuphfCount = await _context.Wuphfs.CountAsync();
        return Page();
    }

    public async Task<IActionResult> OnGetRemoveAsync(int id)
    {
        if (!IsAdmin()) return RedirectToPage("/Admin/Login");

        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }

    public IActionResult OnPostSignOut()
    {
        Response.Cookies.Delete("wuphf_admin");
        return RedirectToPage("/Admin/Login");
    }

    private bool IsAdmin() => Request.Cookies["wuphf_admin"] == "admin";
}
