using System.Reflection;

namespace Wordlie.Infrastructure;

public static class GlobalGame
{
    public static Word DailyWord { get; set; } = new ("кисуся");
    
    public static Dictionary<Guid, Party> PartiesMap { get; set; } = new ();
    
    public static State State { get; set; }

    public static (string atts, bool success) CheckWord(string word, Guid gameId)
    {
        if (!PartiesMap.TryGetValue(gameId, out var currentParty))
            return ("Bad Id", false);

        var resultWordArray = new Letter[word.Length];
        var currentWordToString = string.Concat(currentParty.CurrentWord.LetterArray.Select(x => x.LetterValue));
        for (var i = 0; i < word.Length; i++)
        {
            var currentLetter = word[i];
            if (currentWordToString.Contains(currentLetter) &&
                !currentWordToString.IndexesChar(currentLetter).Contains(i))
            {
                resultWordArray[i] = new Letter(currentLetter, isMoved: true);
                continue;
            }

            if (currentWordToString.Contains(currentLetter) &&
                currentWordToString.IndexesChar(currentLetter).Contains(i))
            {
                resultWordArray[i] = new Letter(currentLetter, isKnown:true);
                continue;
            }
            
            resultWordArray[i] = new Letter(currentLetter);
        }
        
        var resultWord = new Word(resultWordArray).ToString();
        var success = resultWord == currentParty.CurrentWord.WordString;
        currentParty.Attempts.Add(resultWord);

        if (!success)
        {
            var unscsatts = new string[currentParty.Attempts.Count];
            currentParty.Attempts.CopyTo(unscsatts, 0);
            
            return (string.Join("\n", unscsatts.Reverse()), success);
        }
        
        currentParty.Attempts.Add($"Поздравляем! Вы угадали слово {resultWord}");
        var attempts = new string[currentParty.Attempts.Count];
        currentParty.Attempts.CopyTo(attempts, 0);
        if (success)
            currentParty.Attempts.Clear();
        return (string.Join("\n", attempts.Reverse()), success);
    }
    
}