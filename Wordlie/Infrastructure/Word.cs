namespace Wordlie.Infrastructure;

public class Word
{
    internal string WordString { get; set; }
    public IReadOnlyList<Letter> LetterArray { get; init; }

    public Word(string wordString)
    {
        WordString = wordString;
        LetterArray = wordString.Select(letter => new Letter(letter)).ToList();
    }
    
    public Word(Letter[] letterArray)
    {
        WordString = string.Concat(letterArray.Select(letter => letter.Value));
        LetterArray = letterArray;
    }
    
    public static Word GetDifference(Word guess, Word reference)
    {
        return new Word(guess.LetterArray
            .Select(reference.GetLetterState)
            .ToArray());
    }

    private Letter GetLetterState(Letter outerLetter, int position)
    {
        bool isKnown = false, isMoved = true;
        if (LetterArray.Any(letter => letter.Value == outerLetter.Value))
            isKnown = true;
        if (LetterArray[position].Value == outerLetter.Value)
            isMoved = false;
        return new Letter(outerLetter.Value, isKnown, isMoved);

    }

    public override string ToString() => string.Concat(LetterArray
        .Select(letter => letter.IsKnown 
            ? letter.Value 
            : letter.IsMoved ? char.ToUpper(letter.Value) : '*'));

    public Dictionary<string, object> GetJson()
    {
        var json = new Dictionary<string, object>();
        var counter = 0;
        foreach (var letter in LetterArray)
        {
            json[$"Letter {counter++}"] = new
            {
                LetterValue = letter.Value,
                letter.IsKnown,
                letter.IsMoved
            };
        }
        return json;
    }

    public static explicit operator Word(string word)
    {
        var letters = word.Select(letter => new Letter(letter)).ToArray();
        return new Word(letters);
    }
}