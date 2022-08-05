public class ScorePanel : StatsPanel
{
    public override string[] GetValues()
    {
        return gameObject.tag == "white" ? Results.whiteStats.ToArray() : Results.blackStats.ToArray();
    }
}