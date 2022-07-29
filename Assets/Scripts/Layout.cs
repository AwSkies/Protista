using System.Collections;
using System.Collections.Generic;

/// <summary> Class representing a particular custom board layout to be serialized to json and deserialized from json.</summary>
public class Layout
{
    public int Rows { get; set; }
    public int Columns { get; set; }

    public int? ObjectiveHexNum { get; set; }
    public int[][] ObjectiveHexes { get; set; }
    public int? PieceNum { get; set; }
    public List<PieceInfo> Pieces { get; set; }

    public int? Seed { get; set; }

    public string Author { get; set; }
    public string Description { get; set; }
}