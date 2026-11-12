using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WuphfWeb.Data;
using WuphfWeb.Services;

namespace WuphfWeb.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly WuphfDbContext _context;
        private readonly IWuphfService _wuphfService;

        public AdminController(WuphfDbContext context, IWuphfService wuphfService)
        {
            _context = context;
            _wuphfService = wuphfService;
        }

        [HttpGet("SearchUsers")]
        public async Task<IActionResult> SearchUsers(string query)
        {
            if (Request.Cookies["IsAdmin"] != "true")
            {
                return Unauthorized();
            }

            var sql = $"SELECT * FROM Users WHERE Username LIKE '%{query}%'";
            var users = await _context.Users.FromSqlRaw(sql).ToListAsync();

            return Json(users);
        }

        [HttpPost("DeleteWuphf")]
        public async Task<IActionResult> DeleteWuphf(int id)
        {
            var wuphf = await _wuphfService.GetWuphfAsync(id);
            if (wuphf == null)
            {
                return NotFound();
            }

            _context.Wuphfs.Remove(wuphf);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
