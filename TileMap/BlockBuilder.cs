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
        var blueWidth = 0.1;
        bluePoint = bluePoint * size;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size * blueWidth; j++)
            {
                tilemap.SetCell(0, new Vector2I(Math.Abs(bluePoint.X - i), Math.Abs(bluePoint.Y - j)), 3, new Vector2I(0, 0), 0);
                tilemap.SetCell(0, new Vector2I(Math.Abs(bluePoint.X - j), Math.Abs(bluePoint.Y - i)), 3, new Vector2I(0, 0), 0);
            }
        }

        var peerPoint = new Vector2I(size - 1 - bluePoint.X, size - 1 - bluePoint.Y);
        for (int i = 0; i < size * (1 - blueWidth); i++)
        {
            for (int j = 0; j < size * (1 - blueWidth); j++)
            {
                tilemap.SetCell(0, new Vector2I(Math.Abs(peerPoint.X - i), Math.Abs(peerPoint.Y - j)), 0, new Vector2I(0, 0), 0);
                tilemap.SetCell(0, new Vector2I(Math.Abs(peerPoint.X - j), Math.Abs(peerPoint.Y - i)), 0, new Vector2I(0, 0), 0);
            }
        }

        var purpleWidth = 0.25;
        for (int i = 0; i < size * (1 - blueWidth); i++)
        {
            for (int j = 0; j < size * purpleWidth; j++)
            {
                tilemap.SetCell(0, new Vector2I(Math.Abs(peerPoint.X - i), Math.Abs(peerPoint.Y - j)), 2, new Vector2I(0, 0), 0);
                tilemap.SetCell(0, new Vector2I(Math.Abs(peerPoint.X - j), Math.Abs(peerPoint.Y - i)), 2, new Vector2I(0, 0), 0);
            }
        }
    }
}