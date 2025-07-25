using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Wordlie.Infrastructure;
using Wordlie.Services;

namespace Wordlie.Controllers;

// gameId = groupName
[Route("game/{gameId}")]
public class PartyController(IHubContext<GameHub> hubContext, WordService wordService) : Controller
{
    [HttpPost]
    [Route("try/{value}")]
    public async Task<IActionResult> TryAsync(string value, Guid gameId)
    {
        if (!GlobalGame.PartiesMap.TryGetValue(gameId, out var currentParty))
            return BadRequest("Неверный Id игры");
        
        var groupName = gameId.ToString();
        
        if (value.Length != currentParty.CurrentWord.LetterArray.Count)
        {
            await hubContext.Clients.Group(groupName).SendAsync("BadWord", 
                $"Количество букв в слове должно быть равно {currentParty.CurrentWord.LetterArray.Count}");
            return BadRequest();
        }
        
        var isContains = await wordService.ContainsWordAsync(value);
        if (!isContains)
            return BadRequest("Ой, такого слова нет в словаре");
        
        var (words, scs) = GlobalGame.CheckWord(value, gameId);
        if (!scs)
        {
            await hubContext.Clients.Group(groupName).SendAsync("ContinueGame", words);
            return Ok();
        }
        GlobalGame.State = State.Menu;
        await hubContext.Clients.Group(groupName).SendAsync("ContinueGame", words);
        return Ok();
    }

    [HttpGet]
    [Route("attempts")]
    public async Task<IActionResult> ReceiveAttemptsAsync(Guid gameId)
    {
        if (!GlobalGame.PartiesMap.TryGetValue(gameId, out var currentParty))
            return BadRequest("Неверный Id игры");
        var groupName = gameId.ToString();
        await hubContext.Clients.Group(groupName).SendAsync("ReceiveAttempts", string.Join("\n", currentParty.Attempts));
        return Ok();
    }
    
}