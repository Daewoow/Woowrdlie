using Microsoft.AspNetCore.Mvc;
using Wordlie.Infrastructure;
using Wordlie.Infrastructure.Attributes;
using Wordlie.Services;

namespace Wordlie.Controllers;

[Route("words")]
public class WordController(WordService wordService) : Controller
{
    private async Task<string> GetWord() => await wordService.GetRandomWordAsync();

    [HttpGet]
    [Route("random")]
    [NoJsonReturn]
    public IActionResult GetRandomWord()
    {
        return Ok(GetWord());
    }
    
    [HttpGet]
    [Route("currentWord/{gameId:guid}")]
    public IActionResult GetCurrentWord(Guid gameId)
    {
        GlobalGame.State = State.Daily;
        if (!GlobalGame.PartiesMap.TryGetValue(gameId, out var currentParty))
            return BadRequest("Неверный Id игры");
        return currentParty.CurrentWord is null 
            ? NotFound() 
            : Ok(currentParty.CurrentWord.GetJson());
    }

    [HttpGet]
    [Route("dailyWord")]
    public IActionResult GetDailyWord() => GlobalGame.DailyWord is null 
        ? NotFound() 
        : Ok(GlobalGame.DailyWord.GetJson());
}
