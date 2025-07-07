using Microsoft.AspNetCore.Mvc;
using Wordlie.Infrastructure;
using Wordlie.Services;

namespace Wordlie.Controllers;

[Route("words")]
public class WordController(WordService wordService) : Controller
{
    private async Task<string> GetWord() => await wordService.GetRandomWordAsync();

    [HttpGet]
    [Route("random")]
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
            : Ok(string.Concat(currentParty.CurrentWord.WordArray.Select(x => x.LetterValue)));
    }

    [HttpGet]
    [Route("dailyWord")]
    public IActionResult GetDailyWord() => GlobalGame.DailyWord is null 
        ? NotFound() 
        : Ok(string.Concat(GlobalGame.DailyWord.WordArray.Select(x => x.LetterValue)));
}