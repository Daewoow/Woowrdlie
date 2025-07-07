using Microsoft.AspNetCore.Mvc;
using Wordlie.Infrastructure;
using Wordlie.Services;

namespace Wordlie.Controllers;

[Route("game")]
public class GameController(WordService wordService) : Controller
{
    private async Task<string> GetWord() => await wordService.GetRandomWordAsync();
    
    [HttpGet]
    [Route("changeDailyWord")]
    public async Task<IActionResult> ChangeDailyWord()
    {
        var currentWord = GlobalGame.DailyWord;
        var word = await GetWord();
        while (GlobalGame.DailyWord == currentWord)
            GlobalGame.DailyWord = (Word)word;
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
    
    [HttpGet]
    [Route("parties/all")]
    public IActionResult AllParties() => Ok(GlobalGame.PartiesMap.Keys);
}