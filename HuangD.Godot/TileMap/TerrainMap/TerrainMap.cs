using Godot;
using Godot.Collections;

public partial class TerrainMap : TileMap
{
    static Dictionary<HuangD.Sessions.Map.TerrainType, int> tile2sourceId = new()
    {
        { HuangD.Sessions.Map.TerrainType.Land, 0 },
        { HuangD.Sessions.Map.TerrainType.Steppe, 1 },
        { HuangD.Sessions.Map.TerrainType.Mount, 2 },
        { HuangD.Sessions.Map.TerrainType.Water, 3 },
        { HuangD.Sessions.Map.TerrainType.Hill, 4 }
    };

    static Dictionary<HuangD.Sessions.Map.TerrainType, int> tile2LayerId = new()
    {
        { HuangD.Sessions.Map.TerrainType.Land, 1 },
        { HuangD.Sessions.Map.TerrainType.Steppe, 3 },
        { HuangD.Sessions.Map.TerrainType.Mount, 2 },
        { HuangD.Sessions.Map.TerrainType.Water, 0 }
    };

    public override void _Ready()
    {
        var dict = HuangD.Sessions.Map.TerrainBuilder.Build(64, "Test");

        foreach (var pair in dict)
        {
            int layerId = tile2LayerId[pair.Value];
            int sourceId = tile2sourceId[pair.Value];

            this.SetCellEx(layerId, new Vector2I(pair.Key.X, pair.Key.Y), sourceId);
        }
    }
}