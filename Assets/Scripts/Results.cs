public class Results
{
    public static Statistics blackStats = new Statistics{};
    public static Statistics whiteStats = new Statistics{};
    public static bool whiteWinner;
    public static string matchDuration;
    public static int turns;

    public static string[] ToArray()
    {
        return new string[] {matchDuration, turns.ToString()};
    }
}