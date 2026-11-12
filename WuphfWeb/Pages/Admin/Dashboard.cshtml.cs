using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WuphfWeb.Data;
using WuphfWeb.Models;

namespace WuphfWeb.Pages.Admin;

public class DashboardModel : PageModel
{
    private readonly WuphfDbContext _context;

    public DashboardModel(WuphfDbContext context)
    {
        _context = context;
    }

    public List<User> Users { get; set; } = new();
    public List<Wuphf> AllWuphfs { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        // Gate the console on the presence of the admin cookie set at login.
        if (Request.Cookies["IsAdmin"] != "true")
        {
            return RedirectToPage("/Admin/Login");
        }

        Users = await _context.Users.ToListAsync();
        AllWuphfs = await _context.Wuphfs.OrderByDescending(w => w.CreatedAt).ToListAsync();

        return Page();
    }
}
