using WuphfWeb.Models;

namespace WuphfWeb.Services
{
    public interface IWuphfService
    {
        Task<List<Wuphf>> GetRecentWuphfsAsync();
        Task<Wuphf> CreateWuphfAsync(string content, string author, List<string> channels, bool isUrgent = false);
        Task<Wuphf?> GetWuphfAsync(int id);
        Task<bool> LikeWuphfAsync(int id);
        Task<bool> RewuphfAsync(int id);
        Task<WuphfReply> AddReplyAsync(int wuphfId, string content, string author);
    }

    public interface INotificationService
    {
        Task SendNotificationAsync(Wuphf wuphf);
        Task<string> SimulateChannelAsync(string channel, string message);
    }

    public interface IPrinterService
    {
        Task<string> PrintWuphfAsync(string content, string author);
    }

    public interface ISoundService
    {
        Task PlayWoofSoundAsync();
    }
}
