namespace Wordlie.Infrastructure;

public class Word
{
    internal string WordString { get; set; }
    public IReadOnlyCollection<Letter> WordArray { get; init; } = [];

    public Word(string wordString)
    {
        WordString = wordString;
        WordArray = wordString.Select(letter => new Letter(letter)).ToList();
    }
    
    public Word(Letter[] wordArray)
    {
        WordString = string.Concat(wordArray.Select(letter => letter.LetterValue));
        WordArray = wordArray;
    }

    public override string ToString() => string.Concat(WordArray
        .Select(x => x.IsKnown 
            ? x.LetterValue 
            : x.IsMoved ? char.ToUpper(x.LetterValue) : '*'));

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