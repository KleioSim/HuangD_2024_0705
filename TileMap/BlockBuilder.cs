using Godot;
using System;

static class BlockBuilder
{
    public static void Build(TileMap tilemap, Random random)
    {
        var bluePoints = new[]
        {
            new Vector2I(0,0),
            new Vector2I(0,1),
            new Vector2I(1,0),
            new Vector2I(1,1)
        };

        var bluePoint = bluePoints[random.Next(0, bluePoints.Length)];

        Build(tilemap, 64, bluePoint);
    }

    private static void Build(TileMap tilemap, int size, Vector2I bluePoint)
    {
        bluePoint = bluePoint * size;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size * 0.1; j++)
            {
                tilemap.SetCell(0, new Vector2I(Math.Abs(bluePoint.X - i), Math.Abs(bluePoint.Y - j)), 3, new Vector2I(0, 0), 0);
                tilemap.SetCell(0, new Vector2I(Math.Abs(bluePoint.X - j), Math.Abs(bluePoint.Y - i)), 3, new Vector2I(0, 0), 0);
            }
        }

        var yellowPoint = new Vector2I(size - 1 - bluePoint.X, size - 1 - bluePoint.Y);

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size * 0.1; j++)
            {
                tilemap.SetCell(0, new Vector2I(Math.Abs(yellowPoint.X - i), Math.Abs(yellowPoint.Y - j)), 1, new Vector2I(0, 0), 0);
                tilemap.SetCell(0, new Vector2I(Math.Abs(yellowPoint.X - j), Math.Abs(yellowPoint.Y - i)), 1, new Vector2I(0, 0), 0);
            }
        }
    }
}