using Microsoft.AspNetCore.Mvc;
using Wordlie.Infrastructure;

namespace Wordlie.Controllers;

[Route("words")]
public class WordController : Controller
{
    public static string GetWord()
    {
        var randomIndex = new Random().Next(0, WordsDictionary.Words.Count);
        return WordsDictionary.Words[randomIndex];
    }
    
    [HttpGet]
    [Route("all")]
    public IActionResult GetAllWords()
    {
        return Ok(WordsDictionary.Words);
    }

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