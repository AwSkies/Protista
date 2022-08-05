public class Statistics
{
    public int PiecesTaken { get; set; }
    public int MovesTaken { get; set; }
    public int HexesTraveled { get; set; }
    public int ObjectiveHexesOccupied { get; set; }

    public string[] ToArray()
    {
        return new string[] {PiecesTaken.ToString(), MovesTaken.ToString(), HexesTraveled.ToString(), ObjectiveHexesOccupied.ToString()};
    }
}