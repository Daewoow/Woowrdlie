using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Wordlie.Infrastructure;

public class GameHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> _connectionUsers = new();
    
    public async Task Send(string message, Guid gameId, string userName)
    {
        if (userName is null) 
            userName = Context.ConnectionId;
        
        var group = gameId.ToString();
        var context = Context.GetHttpContext();
        Console.WriteLine($"HttpContext: {context}");
        await Clients.Group(group).SendAsync("Receive", message, userName);
    }
    
    public override async Task OnConnectedAsync()
    {
        var context = Context.GetHttpContext();
        if (context is not null)
        {
            Console.WriteLine($"UserAgent = {context.Request.Headers.UserAgent}");
            Console.WriteLine($"RemoteIpAddress = {context.Connection?.RemoteIpAddress?.ToString()}");
 
            await base.OnConnectedAsync();
        }
    }
    
    public async Task JoinGame(Guid gameId, string userName)
    {
        _connectionUsers[Context.ConnectionId] = userName;
        
        if (!GlobalGame.PartiesMap.TryGetValue(gameId, out var value))
            throw new HubException("Игра не найдена!");

        var group = gameId.ToString();
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
        
        Context.Items["username"] = userName;
        
        value.Players.Add(new Player {
            ConnectionId = Context.ConnectionId,
            Name = userName
        });

        await Clients.Group(group).SendAsync("PlayerJoined", $"{userName} вошёл");
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_connectionUsers.TryRemove(Context.ConnectionId, out var username)) 
            await Clients.All.SendAsync("Notify", $"{username} вышел");
        await base.OnDisconnectedAsync(exception);
    }
}