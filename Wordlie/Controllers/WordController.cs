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
    [Route("currentWord")]
    public IActionResult GetCurrentWord() => GameState.CurrentWord is null 
        ? NotFound() 
        : Ok(string.Concat(GameState.CurrentWord.WordArray.Select(x => x.LetterValue)));
    
    [HttpGet]
    [Route("dailyWord")]
    public IActionResult GetDailyWord() => GameState.DailyWord is null 
        ? NotFound() 
        : Ok(string.Concat(GameState.DailyWord.WordArray.Select(x => x.LetterValue)));
}