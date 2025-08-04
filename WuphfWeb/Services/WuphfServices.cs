using Microsoft.EntityFrameworkCore;
using WuphfWeb.Data;
using WuphfWeb.Models;

namespace WuphfWeb.Services
{
    public class WuphfService : IWuphfService
    {
        private readonly WuphfDbContext _context;

        public WuphfService(WuphfDbContext context)
        {
            _context = context;
        }

        public async Task<List<Wuphf>> GetRecentWuphfsAsync()
        {
            return await _context.Wuphfs
                .Include(w => w.Replies)
                .OrderByDescending(w => w.CreatedAt)
                .Take(50)
                .ToListAsync();
        }

        public async Task<Wuphf> CreateWuphfAsync(string content, string author, List<string> channels, bool isUrgent = false)
        {
            var wuphf = new Wuphf
            {
                Content = content,
                AuthorName = author,
                NotificationChannels = channels,
                IsUrgent = isUrgent,
                CreatedAt = DateTime.Now
            };

            _context.Wuphfs.Add(wuphf);
            await _context.SaveChangesAsync();

            return wuphf;
        }

        public async Task<Wuphf?> GetWuphfAsync(int id)
        {
            return await _context.Wuphfs
                .Include(w => w.Replies)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<bool> LikeWuphfAsync(int id)
        {
            var wuphf = await _context.Wuphfs.FindAsync(id);
            if (wuphf == null) return false;

            wuphf.Likes++;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RewuphfAsync(int id)
        {
            var wuphf = await _context.Wuphfs.FindAsync(id);
            if (wuphf == null) return false;

            wuphf.Rewuphfs++;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<WuphfReply> AddReplyAsync(int wuphfId, string content, string author)
        {
            var reply = new WuphfReply
            {
                WuphfId = wuphfId,
                Content = content,
                AuthorName = author,
                CreatedAt = DateTime.Now
            };

            _context.WuphfReplies.Add(reply);
            await _context.SaveChangesAsync();

            return reply;
        }
    }

    public class NotificationService : INotificationService
    {
        public async Task SendNotificationAsync(Wuphf wuphf)
        {
            // Simulate sending notifications to all channels
            await Task.Delay(100); // Simulate processing time
        }

        public async Task<string> SimulateChannelAsync(string channel, string message)
        {
            await Task.Delay(500); // Simulate processing time

            return channel.ToLower() switch
            {
                "email" => $"📧 Email sent to all contacts: {message}",
                "sms" => $"📱 Text message broadcast: {message}",
                "facebook" => $"📘 Posted to Facebook wall: {message}",
                "twitter" => $"🐦 Tweeted: {message}",
                "printer" => $"🖨️ Printing WUPHF on nearest printer: {message}",
                "homephone" => $"☎️ Calling all home phones: {message}",
                "pager" => $"📟 Pager alert sent: {message}",
                "fax" => $"📠 Fax transmitted: {message}",
                _ => $"📢 {channel} notification: {message}"
            };
        }
    }

    public class PrinterService : IPrinterService
    {
        public async Task<string> PrintWuphfAsync(string content, string author)
        {
            await Task.Delay(2000); // Simulate printer delay

            var printOutput = $@"
════════════════════════════════════════
                 W U P H F !
════════════════════════════════════════
From: {author}
Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}

{content}

════════════════════════════════════════
    WUPHF - The Future of Communication
         washington university 
        public health fund
════════════════════════════════════════
";
            return printOutput;
        }
    }

    public class SoundService : ISoundService
    {
        public async Task PlayWoofSoundAsync()
        {
            // In a real app, this would play an actual sound file
            await Task.Delay(1000); // Simulate sound playing
        }
    }
}
