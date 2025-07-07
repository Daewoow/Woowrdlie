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
        var currentWord = GlobalGame.DailyWord;
        while (GlobalGame.DailyWord == currentWord)
            GlobalGame.DailyWord = (Word)WordController.GetWord();
        return Ok();
    }

    [HttpGet]
    [Route("startDaily")]
    public IActionResult StartDaily()
    {
        var newParty = new Party();
        var gameId = newParty.GameId;
        GlobalGame.PartiesMap[gameId] = newParty;
        newParty.CurrentWord = GlobalGame.DailyWord;
        return Ok(new
        {
            id = gameId,
            word = newParty.CurrentWord.ToString()
        });
    }
    
    // public IActionResult JoinDaily(Guid gameId)
    
    [HttpGet]
    [Route("parties/all")]
    public IActionResult AllParties() => Ok(GlobalGame.PartiesMap.Keys);
}