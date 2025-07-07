namespace Wordlie.Infrastructure;

public class Party
{
    public Guid GameId { get; set; } = Guid.NewGuid(); // GameId = GroupName
    
    public Word CurrentWord { get; set; } = GlobalGame.DailyWord;
    public readonly List<Player> Players = new ();
    public List<string> Attempts { get; } = [];

    public Party()
    {
        Attempts.Add(CurrentWord.ToString());
    }
}