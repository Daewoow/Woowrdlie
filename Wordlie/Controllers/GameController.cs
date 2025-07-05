using Microsoft.AspNetCore.Mvc;
using Wordlie.Infrastructure;

namespace Wordlie.Controllers;

[Route("game")]
public class GameController : Controller
{
    [HttpGet]
    [Route("changeDailyWord")]
    public IActionResult ChangeDailyWord()
    {
        var currentWord = GameState.DailyWord;
        while (GameState.DailyWord == currentWord)
            GameState.DailyWord = (Word)WordController.GetWord();
        return Ok();
    }

    [HttpPost]
    [Route("startDaily")]
    public IActionResult StartDaily()
    {
        GameState.State = State.Daily;
        GameState.CurrentWord = GameState.DailyWord;
        return Ok(GameState.DailyWord.ToString());
    }

    [HttpPost]
    [Route("try/{value}")]
    public IActionResult Try(string value)
    {
        if (value.Length != GameState.CurrentWord.WordArray.Count)
            return BadRequest($"Количество букв в слове должно быть равно {GameState.CurrentWord.WordArray.Count}");
        var (words, scs) = GameState.CheckWord(value);
        if (!scs) 
            return Ok(words);
        GameState.State = State.Menu;
        return Ok(words);
    }
}