public class Statistics
{
    public int PiecesTaken { get; set; } = 0;
    public int MovesTaken { get; set; } = 0;
    public int HexesTraveled { get; set; } = 0;
    public int ObjectiveHexesOccupied { get; set; }

    public string[] ToArray()
    {
        return new string[] {PiecesTaken.ToString(), MovesTaken.ToString(), HexesTraveled.ToString(), ObjectiveHexesOccupied.ToString()};
    }
}