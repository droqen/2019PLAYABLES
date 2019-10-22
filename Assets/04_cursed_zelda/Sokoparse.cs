using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;

public static class Sokoparse
{
    public static List<Sokolevel> ParseFullAsset(TextAsset textAsset)
    {
        List<Sokolevel> levels = new List<Sokolevel>();
        foreach(var levelString in textAsset.text.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries))
        {
            levels.Add(ParseLevelBlock(levelString));
        }
        return levels;
    }
    public static Sokolevel ParseLevelBlock(string levelBlock)
    {
        var rows = levelBlock.Split('\n');
        int width = 0;
        foreach(var row in rows) width = Mathf.Max(width, row.Length);
        for(var i=0;i<rows.Length;i++) if (rows[i].Length < width) rows[i] += new string('\'', width - rows[i].Length);

        //// cut off top and bottom and left and right.
        //var shrunkrows = new string[rows.Length - 2];
        //for(var y=0;y<shrunkrows.Length;y++)
        //{
        //    shrunkrows[y] = rows[y + 1].Substring(1, width - 2);
        //}

        return new Sokolevel(rows);
    }
}

public static class Sokopack
{
    public static List<twin> GetPackedLevelLocations(List<Sokolevel> levels)
    {
        var levelLocations = new List<twin>();
        levelLocations.Add(twin.zero);
        for (int i = 1; i < levels.Count; i++)
        {
            levelLocations.Add(DecideNextLocation(i, levelLocations, levels));
        }
        return levelLocations;
    }

    static twin DecideNextLocation(int index, List<twin> levelLocations, List<Sokolevel> levels)
    {
        return levelLocations[index - 1] + twin.right * levels[index - 1].width;
    }
}

public class Sokolevel
{
    public enum Tile
    {
        Wall, Floor, Boulder, Goal, Player,
    }
    public int width, height;
    public Tile[,] tileGrid;
    internal Sokolevel(string[] rows)
    {
        this.width = rows[0].Length;
        this.height = rows.Length;
        tileGrid = new Tile[width, height];
        for(var y=0;y<height;y++)
        {
            for(var x=0;x<width;x++)
            {
                tileGrid[x, y] = Tile.Floor;
                switch (rows[y][x])
                {
                    case '#': tileGrid[x, y] = Tile.Wall; break;
                    case '$': tileGrid[x, y] = Tile.Boulder; break;
                    case '@': tileGrid[x, y] = Tile.Player; break;
                    case '.': tileGrid[x, y] = Tile.Goal; break;
                }
            }
        }
    }

    public void DoEach(twin pos, System.Action<twin, Tile> doWithTile)
    {
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                doWithTile(pos + new twin(x, y), tileGrid[x, y]);
            }
        }
    }
}
