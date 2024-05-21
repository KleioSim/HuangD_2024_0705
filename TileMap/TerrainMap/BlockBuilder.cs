using Godot;
using System;

static class BlockBuilder
{
    public static void Build(TileMap tilemap, Random random)
    {
        var startPoints = new[]
        {
            new Vector2I(0,0),
            new Vector2I(0,1),
            new Vector2I(1,0),
            new Vector2I(1,1)
        };

        var startPoint = startPoints[random.Next(0, startPoints.Length)];

        Build(tilemap, 64, startPoint, random);
    }

    private static void Build(TileMap tilemap, int size, Vector2I startPoint, Random random)
    {

        var landRate = 0.9;
        var mountRate = 0.3;

        startPoint = startPoint * size;

        for(int i=0; i<size; i++)
        {
            for(int j=0; j<size; j++)
            {
                tilemap.SetCell(0, new Vector2I(i, j), 3, new Vector2I(0, 0), 0);
            }
        }

        for (int i = 0; i < size * landRate; i++)
        {
            for (int j = 0; j < size * landRate; j++)
            {
                tilemap.SetCell(1, new Vector2I(Math.Abs(startPoint.X - i), Math.Abs(startPoint.Y - j)), 0, new Vector2I(0, 0), 0);
                tilemap.SetCell(1, new Vector2I(Math.Abs(startPoint.X - j), Math.Abs(startPoint.Y - i)), 0, new Vector2I(0, 0), 0);
            }
        }

        for (int i = 0; i < size * landRate; i++)
        {
            for (int j = 0; j < size * mountRate; j++)
            {
                tilemap.SetCell(2, new Vector2I(Math.Abs(startPoint.X - i), Math.Abs(startPoint.Y - j)), 2, new Vector2I(0, 0), 0);
                tilemap.SetCell(2, new Vector2I(Math.Abs(startPoint.X - j), Math.Abs(startPoint.Y - i)), 2, new Vector2I(0, 0), 0);
            }
        }


        var row = 0.7;
        var colum = 0.5;

        if(random.Next(0,2) == 0)
        {
            row = 0.5;
            colum = 0.7;
        }

        for (int j = 0; j < size * mountRate; j++)
        {
            for (int i = 0; i < size * landRate * row; i++)
            {
                tilemap.SetCell(3, new Vector2I(Math.Abs(startPoint.X - j), Math.Abs(startPoint.Y - i)), 1, new Vector2I(0, 0), 0);
            }
        }

        for (int i = 0; i < size * mountRate; i++)
        {
            for (int j = 0; j < size * landRate * colum; j++)
            {
                tilemap.SetCell(3, new Vector2I(Math.Abs(startPoint.X - j), Math.Abs(startPoint.Y - i)), 1, new Vector2I(0, 0), 0);
            }
        }
    }
}