namespace Wordlie.Infrastructure;

public static class StringExtensions
{
    public static IEnumerable<int> IndexesChar(this string str, char ch)
    {
        var indexes = new List<int>();
        var index = -1;
        while ((index = str.IndexOf(ch, index + 1)) != -1)
        {
            indexes.Add(index);
        }
        
        return indexes;
    }
}
