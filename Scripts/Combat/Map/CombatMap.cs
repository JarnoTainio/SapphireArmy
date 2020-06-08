using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMap : MonoBehaviour
{
    [Header("Properties")]
    public int width = 8;
    public int height = 8;

    [Header("References")]
    public CombatManager combatManager;
    public UnitManager unitManager;

    [Header("Prefabs")]
    public Tile tilePrefab;
    public Tile[,] tiles;

    [Header("Markings")]
    public Sprite[] markings;

    public void Load(string data)
    {
        string[] parts = data.Split();
        int i = 0;
        width = int.Parse(parts[i++]);
        height = int.Parse(parts[i++]);

        tiles = new Tile[width, height];
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = Instantiate(tilePrefab, transform);
                tile.transform.localPosition = new Vector2(x, y);
                tiles[x, y] = tile;
                tile.SetMap(this);
            }
        }
    }

    public void Load(Combat combat)
    {
        width = combat.width;
        height = combat.height;

        tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = Instantiate(tilePrefab, transform);
                tile.transform.localPosition = new Vector2(x, y);
                tiles[x, y] = tile;
                tile.SetMap(this);
            }
        }
    }

    public void TileClicked(Point tile)
    {
        combatManager.TileClicked(tile);
    }

    public void TileMouseEnter(Point tile)
    {
        combatManager.TileHover(tile);
    }

    public void TileMouseExit(Point tile)
    {
        combatManager.TileHoverEnd(tile);
    }

    public void Mark(Point tile, Marking mark)
    {
        if (IsWithinBounds(tile))
        {
            tiles[tile.x, tile.y].SetMarking(markings[(int)mark]);
        }
    }

    public void RemoveMarkings()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y].HideMarking();
            }
        }
    }

    public void RemoveMarkings(List<Point> points)
    {
        foreach(Point p in points)
        {
            tiles[p.x, p.y].HideMarking();
        }
    }

    public bool IsWithinBounds(Point point)
    {
        if (point.x < 0 || point.y < 0 || point.x >= width || point.y >= height)
        {
            return false;
        }
        return true;
    }

    public override string ToString()
    {
        string str = width + " " + height;
        str += "\n";
        return str;
    }
}

public enum Marking { Green = 0, Red, Orange, Purple, Energy}
