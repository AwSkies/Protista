/// <summary>The information about a piece to be serialized to json</summary>
public class PieceInfo
{
    public int[] Position { get; set; }
    public int Stacked { get; set; }
    public bool White { get; set; }
}