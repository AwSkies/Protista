/// <summary> Class representing a particular custom board layout to be serialized to json and deserialized from json.</summary>
public class Layout
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public (int Z, int X)[] ObjectiveHexes { get; set; }
    public (int Z, int X, int Stack)[] BlackPieces { get; set; }
    public (int Z, int X, int Stack)[] WhitePieces { get; set; }

    public string author;
    public string description;
}