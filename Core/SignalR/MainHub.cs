using Core;
using Core.SignalR;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace SignalRApp;

public class MainHub : Hub
{

    private static readonly Dictionary<string, ClientData> ConnectedClients = new Dictionary<string, ClientData>();

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine(" im connected");
        await Clients.All.SendAsync("mounted", $"{Context.ConnectionId}");

        string connectionId = Context.ConnectionId;
        ClientsService.ConnectedClients[connectionId] = new ClientData();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        string connectionId = Context.ConnectionId;
        ClientsService.ConnectedClients.Remove(connectionId);
        await Clients.Client(connectionId).SendAsync("onDisconnect");
        await base.OnDisconnectedAsync(exception);
 
    }

    /// <summary>
    /// Sends the response to client.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="message">The message.</param>
    public async Task SendResponseToClient(string clientId, string message)
    {
        await Clients.Client(clientId).SendAsync("exchanger", message);
    }

    public async Task ClientChangedData(string changedInfo)
    {
        string connectionId = Context.ConnectionId;
        var clientData = JsonSerializer.Deserialize<ClientData>(changedInfo);
        ClientsService.ConnectedClients[connectionId] = clientData;
    }
}