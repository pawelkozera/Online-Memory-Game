using Microsoft.AspNetCore.SignalR.Client;

var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5296/chatHub")
    .WithAutomaticReconnect()
    .Build();

connection.On<string, string>("ReceiveMessage", (user, message) =>
{
    Console.WriteLine($"{user}: {message}");
});

try
{
    await connection.StartAsync();
    Console.WriteLine("Połączono z serwerem SignalR.");
}
catch (Exception ex)
{
    Console.WriteLine($"Błąd połączenia: {ex.Message}");
    return;
}

while (true)
{
    Console.Write("Podaj nazwę użytkownika: ");
    string user = Console.ReadLine();

    Console.Write("Podaj wiadomość: ");
    string message = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(message)) break;

    await connection.InvokeAsync("SendMessage", user, message);
}

await connection.StopAsync();
