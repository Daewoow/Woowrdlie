namespace Wordlie.Infrastructure;

public class Letter(char letter, bool isKnown = false, bool isMoved = false)
{
    public char LetterValue { get; init; } = letter;
    public bool IsKnown { get; internal set; } = isKnown;
    public bool IsMoved { get; internal set; } = isMoved;
}