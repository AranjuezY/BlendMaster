using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace WebApplication2.Services
{
    public class NotificationHub : Hub
    {
        // Dictionaries to store connection ids by role
        private static ConcurrentDictionary<string, string> Administrators = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<string, string> Customers = new ConcurrentDictionary<string, string>();

        // Method to handle connections and assign roles
        public override Task OnConnectedAsync()
        {
            // Assume roles are assigned via query string parameters for simplicity
            var role = Context.GetHttpContext().Request.Query["role"];

            if (role == "administrator")
            {
                Administrators.TryAdd(Context.ConnectionId, Context.ConnectionId);
            }
            else if (role == "customer")
            {
                Customers.TryAdd(Context.ConnectionId, Context.ConnectionId);
            }

            return base.OnConnectedAsync();
        }

        // Method to handle disconnections and remove roles
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Administrators.TryRemove(Context.ConnectionId, out _);
            Customers.TryRemove(Context.ConnectionId, out _);

            return base.OnDisconnectedAsync(exception);
        }

        // Method for customers to send notifications
        public Task SendNotification(string message)
        {
            foreach (var administrator in Administrators)
            {
                Clients.Client(administrator.Key).SendAsync("notify", message);
            }

            return Task.CompletedTask;
        }
    }
}
