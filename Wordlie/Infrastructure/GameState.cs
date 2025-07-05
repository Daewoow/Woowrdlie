namespace Wordlie.Infrastructure;

public static class GameState
{
    public static Word DailyWord { get; set; } = new ("кисуся");
    
    public static Word CurrentWord { get; set; } = DailyWord;
    public static List<string> Attempts { get; } = [];
    
    public static State State { get; set; }

    public static (string atts, bool success) CheckWord(string word)
    {
        var resultWordArray = new Letter[word.Length];
        var currentWordToString = string.Concat(CurrentWord.WordArray.Select(x => x.LetterValue));
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
        var success = resultWord == CurrentWord.WordString;
        Attempts.Add(resultWord);
        
        if (!success) 
            return (string.Join("\n", Attempts), success);
        
        Attempts.Add($"Поздравляем! Вы угадали слово {resultWord}");
        Attempts.Clear();
        
        return (string.Join("\n", Attempts), success);
    }
    
}