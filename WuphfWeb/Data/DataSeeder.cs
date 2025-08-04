using WuphfWeb.Data;
using WuphfWeb.Models;

namespace WuphfWeb.Data
{
    public class DataSeeder
    {
        private readonly WuphfDbContext _context;

        public DataSeeder(WuphfDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (_context.Users.Any()) return; // Already seeded

            // Seed notification channels
            var channels = new List<NotificationChannel>
            {
                new() { Name = "Email", Description = "Traditional electronic mail", IconClass = "fas fa-envelope", IsActive = true },
                new() { Name = "SMS", Description = "Text messaging", IconClass = "fas fa-sms", IsActive = true },
                new() { Name = "Facebook", Description = "Facebook wall post", IconClass = "fab fa-facebook", IsActive = true },
                new() { Name = "Twitter", Description = "Tweet notification", IconClass = "fab fa-twitter", IsActive = true },
                new() { Name = "Printer", Description = "Physical printout", IconClass = "fas fa-print", IsActive = true },
                new() { Name = "HomePhone", Description = "Landline call", IconClass = "fas fa-phone", IsActive = true },
                new() { Name = "Pager", Description = "90s technology at its finest", IconClass = "fas fa-pager", IsActive = true },
                new() { Name = "Fax", Description = "Because it's 1987 somewhere", IconClass = "fas fa-fax", IsActive = true }
            };

            _context.NotificationChannels.AddRange(channels);

            // Seed users
            var users = new List<User>
            {
                new() { Username = "RyanTheWUPHF", Email = "ryan@wuphf.com", PhoneNumber = "555-WUPHF", WuphfsSent = 847, WuphfsReceived = 12 },
                new() { Username = "MichaelScott", Email = "michael@dundermifflin.com", PhoneNumber = "555-BOSS", WuphfsSent = 234, WuphfsReceived = 456 },
                new() { Username = "DwightKSchrute", Email = "dwight@schrutefarms.com", PhoneNumber = "555-BEET", WuphfsSent = 156, WuphfsReceived = 89 },
                new() { Username = "JimHalpert", Email = "jim@dundermifflin.com", PhoneNumber = "555-PRANK", WuphfsSent = 98, WuphfsReceived = 234 },
                new() { Username = "PamBeesly", Email = "pam@dundermifflin.com", PhoneNumber = "555-ART", WuphfsSent = 67, WuphfsReceived = 123 }
            };

            _context.Users.AddRange(users);

            // Seed sample wuphfs
            var wuphfs = new List<Wuphf>
            {
                new() 
                { 
                    Content = "🚀 WUPHF is LIVE! The future of communication is HERE! Email, text, call, Facebook, Twitter, printer - ALL AT ONCE! #GameChanger #WUPHF", 
                    AuthorName = "RyanTheWUPHF", 
                    Likes = 47, 
                    Rewuphfs = 12,
                    NotificationChannels = new() { "Email", "SMS", "Facebook", "Twitter", "Printer" },
                    IsUrgent = true,
                    CreatedAt = DateTime.Now.AddMinutes(-45)
                },
                new() 
                { 
                    Content = "That's what she said! 😂 Had to WUPHF this classic moment from today's meeting!", 
                    AuthorName = "MichaelScott", 
                    Likes = 23, 
                    Rewuphfs = 8,
                    NotificationChannels = new() { "Email", "Facebook" },
                    CreatedAt = DateTime.Now.AddMinutes(-30)
                },
                new() 
                { 
                    Content = "FACT: Bears eat beets. Bears. Beets. Battlestar Galactica. WUPHF makes communication as efficient as a well-organized beet farm. 🐻", 
                    AuthorName = "DwightKSchrute", 
                    Likes = 15, 
                    Rewuphfs = 4,
                    NotificationChannels = new() { "Email", "SMS", "Printer" },
                    CreatedAt = DateTime.Now.AddMinutes(-20)
                },
                new() 
                { 
                    Content = "Just put Dwight's stapler in Jell-O again. WUPHF'd the whole office about it! 🍮📎", 
                    AuthorName = "JimHalpert", 
                    Likes = 34, 
                    Rewuphfs = 15,
                    NotificationChannels = new() { "Facebook", "Twitter" },
                    CreatedAt = DateTime.Now.AddMinutes(-10)
                },
                new() 
                { 
                    Content = "Working on some new art pieces! WUPHF makes it easy to share my progress with everyone instantly! 🎨", 
                    AuthorName = "PamBeesly", 
                    Likes = 19, 
                    Rewuphfs = 3,
                    NotificationChannels = new() { "Email", "Facebook" },
                    CreatedAt = DateTime.Now.AddMinutes(-5)
                }
            };

            _context.Wuphfs.AddRange(wuphfs);
            await _context.SaveChangesAsync();
        }
    }
}
