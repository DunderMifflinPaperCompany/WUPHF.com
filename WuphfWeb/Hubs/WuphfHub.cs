using Microsoft.AspNetCore.SignalR;

namespace WuphfWeb.Hubs
{
    public class WuphfHub : Hub
    {
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("UserJoined", Context.ConnectionId);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("UserLeft", Context.ConnectionId);
        }

        public async Task SendWuphf(string user, string message, string[] channels)
        {
            await Clients.All.SendAsync("ReceiveWuphf", user, message, channels);
        }

        public async Task SendNotification(string user, string message, string channel)
        {
            await Clients.All.SendAsync("ReceiveNotification", user, message, channel);
        }

        public async Task TriggerPrint(string message)
        {
            await Clients.All.SendAsync("PrintWuphf", message);
        }

        public async Task PlayWoofSound()
        {
            await Clients.All.SendAsync("PlayWoof");
        }
    }
}
