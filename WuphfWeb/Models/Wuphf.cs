using System.ComponentModel.DataAnnotations;

namespace WuphfWeb.Models
{
    public class Wuphf
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(280, ErrorMessage = "Wuphfs must be 280 characters or less")]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        public string AuthorName { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public int Likes { get; set; } = 0;
        
        public int Rewuphfs { get; set; } = 0;
        
        public List<string> NotificationChannels { get; set; } = new();
        
        public bool IsPrinted { get; set; } = false;
        
        public bool IsUrgent { get; set; } = false;
        
        public string? ImageUrl { get; set; }
        
        public List<WuphfReply> Replies { get; set; } = new();
    }

    public class WuphfReply
    {
        public int Id { get; set; }
        public int WuphfId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Wuphf? Wuphf { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? FacebookId { get; set; }
        public string? TwitterHandle { get; set; }
        public bool EnablePrinterNotifications { get; set; } = true;
        public bool EnableSoundNotifications { get; set; } = true;
        public bool EnableEmailNotifications { get; set; } = true;
        public bool EnableSmsNotifications { get; set; } = true;
        public DateTime JoinedAt { get; set; } = DateTime.Now;
        public int WuphfsSent { get; set; } = 0;
        public int WuphfsReceived { get; set; } = 0;
    }

    public class NotificationChannel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string IconClass { get; set; } = string.Empty;
    }
}
