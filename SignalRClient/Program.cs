using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Text.Json;
using System.Threading.Tasks;

var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5296/gameHub")
    .WithAutomaticReconnect()
    .Build();

try
{
    await connection.StartAsync();
    Console.WriteLine("Połączono z serwerem.");
}
catch (Exception ex)
{
    Console.WriteLine($"Błąd połączenia: {ex.Message}");
    return;
}

connection.On<string>("ReceiveResponse", (jsonResponse) =>
{
    Console.WriteLine($"Odebrana odpowiedź JSON: {jsonResponse}");

    var response = JsonSerializer.Deserialize<GameResponse>(jsonResponse);

    if (response != null && response.Status == "success")
    {
        Console.WriteLine($"Dołączono do gry {response.GameId}");
        Console.WriteLine("Gracze w grze: " + string.Join(", ", response.Players));
    }
    else
    {
        Console.WriteLine($"Błąd: {response?.Status}");
    }
});


Console.Write("Podaj swój nick: ");
string playerName = Console.ReadLine();

var request = new
{
    action = "join",
    playerId = Guid.NewGuid().ToString(),
    playerName = playerName
};

string jsonRequest = JsonSerializer.Serialize(request);

Console.WriteLine($"Wysyłany JSON: {jsonRequest}");

await connection.SendAsync("HandleRequest", jsonRequest);

Console.WriteLine("Wysłano żądanie do serwera!");

Console.ReadLine();
await connection.StopAsync();

public class GameResponse
{
    public string Status { get; set; }
    public string GameId { get; set; }
    public List<string> Players { get; set; }
}
