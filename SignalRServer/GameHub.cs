using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SignalRServer
{
    public class GameHub : Hub
    {
        private readonly ILogger<GameHub> _logger;
        private static Dictionary<string, List<string>> _games = new();

        public GameHub(ILogger<GameHub> logger)
        {
            _logger = logger;
        }

        public async Task HandleRequest(string jsonRequest)
        {
            try
            {
                var request = JsonSerializer.Deserialize<GameRequest>(jsonRequest);

                if (request == null || request.Action != "join")
                {
                    var errorResponse = new { status = "error", message = "Invalid request" };
                    _logger.LogWarning("Nieprawidłowe żądanie: {Request}", jsonRequest);
                    await Clients.Caller.SendAsync("ReceiveResponse", JsonSerializer.Serialize(errorResponse));
                    return;
                }

                string gameId = "1";

                if (!_games.ContainsKey(gameId))
                {
                    _games[gameId] = new List<string>();
                }

                if (!_games[gameId].Contains(request.PlayerName))
                {
                    _games[gameId].Add(request.PlayerName);
                }

                _logger.LogInformation("{PlayerName} dołączył do gry {GameId}", request.PlayerName, gameId);

                var response = new GameResponse
                {
                    Status = "success",
                    GameId = gameId,
                    Players = _games[gameId]
                };

                string jsonResponse = JsonSerializer.Serialize(response);
                _logger.LogInformation("Wysłana odpowiedź: {Response}", jsonResponse);

                await Clients.Caller.SendAsync("ReceiveResponse", jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Błąd: {ex.Message}");
                await Clients.Caller.SendAsync("ReceiveResponse", JsonSerializer.Serialize(new { status = "error", message = "Server error" }));
            }
        }
    }

    public class GameRequest
    {
        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("playerId")]
        public string PlayerId { get; set; }

        [JsonPropertyName("playerName")]
        public string PlayerName { get; set; }
    }

    public class GameResponse
    {
        public string Status { get; set; }
        public string GameId { get; set; }
        public List<string> Players { get; set; }
    }
}
