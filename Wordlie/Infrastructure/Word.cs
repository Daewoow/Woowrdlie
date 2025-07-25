namespace Wordlie.Infrastructure;

public class Word
{
    internal string WordString { get; set; }
    public IReadOnlyCollection<Letter> LetterArray { get; init; }

    public Word(string wordString)
    {
        WordString = wordString;
        LetterArray = wordString.Select(letter => new Letter(letter)).ToList();
    }
    
    public Word(Letter[] letterArray)
    {
        WordString = string.Concat(letterArray.Select(letter => letter.LetterValue));
        LetterArray = letterArray;
    }

    public override string ToString() => string.Concat(LetterArray
        .Select(x => x.IsKnown 
            ? x.LetterValue 
            : x.IsMoved ? char.ToUpper(x.LetterValue) : '*'));

    public Dictionary<string, object> GetJson()
    {
        var json = new Dictionary<string, object>();
        var counter = 0;
        foreach (var letter in LetterArray)
        {
            json[$"Letter {counter++}"] = new
            {
                LetterValue = letter.LetterValue,
                IsKnown = letter.IsKnown,
                IsMoved = letter.IsMoved
            };
        }
        return json;
    }

    public static explicit operator Word(string word)
    {
        var a = new Letter[word.Length];
        for (var i = 0; i < word.Length; i++)
        {
            a[i] = new Letter(word[i]);
        }

        return new Word(a);
    }
}