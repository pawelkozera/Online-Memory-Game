using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace SignalRServer
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public async Task SendMessage(string user, string message)
        {
            Console.WriteLine($"[SERVER] {user}: {message}");

            _logger.LogInformation("Wiadomość od {User}: {Message}", user, message);

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
