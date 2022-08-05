public class MatchPanel : StatsPanel
{
    public override string[] GetValues()
    {
        return Results.ToArray();
    }
}