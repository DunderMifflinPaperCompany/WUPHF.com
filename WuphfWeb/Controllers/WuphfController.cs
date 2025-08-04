using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WuphfWeb.Services;
using WuphfWeb.Hubs;
using WuphfWeb.Models;

namespace WuphfWeb.Controllers
{
    public class WuphfController : Controller
    {
        private readonly IWuphfService _wuphfService;
        private readonly INotificationService _notificationService;
        private readonly IPrinterService _printerService;
        private readonly ISoundService _soundService;
        private readonly IHubContext<WuphfHub> _hubContext;

        public WuphfController(
            IWuphfService wuphfService,
            INotificationService notificationService,
            IPrinterService printerService,
            ISoundService soundService,
            IHubContext<WuphfHub> hubContext)
        {
            _wuphfService = wuphfService;
            _notificationService = notificationService;
            _printerService = printerService;
            _soundService = soundService;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> SendWuphf([FromBody] SendWuphfRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Content is required");
            }

            var wuphf = await _wuphfService.CreateWuphfAsync(
                request.Content, 
                request.Author ?? "Anonymous", 
                request.Channels ?? new List<string>(), 
                request.IsUrgent);

            // Notify all connected clients
            await _hubContext.Clients.All.SendAsync("ReceiveWuphf", wuphf);

            // Play woof sound if enabled
            await _hubContext.Clients.All.SendAsync("PlayWoof");

            // Simulate notifications for each channel
            var notifications = new List<string>();
            foreach (var channel in wuphf.NotificationChannels)
            {
                var notification = await _notificationService.SimulateChannelAsync(channel, wuphf.Content);
                notifications.Add(notification);
            }

            return Json(new { success = true, wuphf = wuphf, notifications = notifications });
        }

        [HttpPost]
        public async Task<IActionResult> LikeWuphf(int id)
        {
            var success = await _wuphfService.LikeWuphfAsync(id);
            if (success)
            {
                await _hubContext.Clients.All.SendAsync("WuphfLiked", id);
            }
            return Json(new { success });
        }

        [HttpPost]
        public async Task<IActionResult> RewuphfWuphf(int id)
        {
            var success = await _wuphfService.RewuphfAsync(id);
            if (success)
            {
                await _hubContext.Clients.All.SendAsync("WuphfRewuphfed", id);
            }
            return Json(new { success });
        }

        [HttpPost]
        public async Task<IActionResult> PrintWuphf(int id)
        {
            var wuphf = await _wuphfService.GetWuphfAsync(id);
            if (wuphf == null)
            {
                return NotFound();
            }

            var printOutput = await _printerService.PrintWuphfAsync(wuphf.Content, wuphf.AuthorName);
            
            // Notify all clients about the print
            await _hubContext.Clients.All.SendAsync("PrintWuphf", printOutput);

            return Json(new { success = true, printOutput });
        }

        [HttpPost]
        public async Task<IActionResult> DemoMode()
        {
            // Create a demo wuphf
            var demoWuphf = await _wuphfService.CreateWuphfAsync(
                "🎉 DEMO MODE ACTIVATED! This is what a WUPHF notification looks like - WOOF! 🐕",
                "WUPHF Demo System",
                new List<string> { "Email", "SMS", "Facebook", "Twitter", "Printer", "HomePhone" },
                true
            );

            // Play woof sound
            await _hubContext.Clients.All.SendAsync("PlayWoof");

            // Send the demo wuphf to all clients
            await _hubContext.Clients.All.SendAsync("ReceiveWuphf", demoWuphf);

            // Simulate all notification channels
            var notifications = new List<string>();
            foreach (var channel in demoWuphf.NotificationChannels)
            {
                var notification = await _notificationService.SimulateChannelAsync(channel, demoWuphf.Content);
                notifications.Add(notification);
                
                // Send individual notifications with delay for effect
                await Task.Delay(500);
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", demoWuphf.AuthorName, notification, channel);
            }

            return Json(new { success = true, wuphf = demoWuphf, notifications });
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentWuphfs()
        {
            var wuphfs = await _wuphfService.GetRecentWuphfsAsync();
            return Json(wuphfs);
        }
    }

    public class SendWuphfRequest
    {
        public string Content { get; set; } = string.Empty;
        public string? Author { get; set; }
        public List<string>? Channels { get; set; }
        public bool IsUrgent { get; set; }
    }
}
