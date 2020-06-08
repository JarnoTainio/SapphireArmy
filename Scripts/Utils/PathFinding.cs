using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class PathFinding
{

    static readonly Point[] lines = new Point[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) };
    static readonly Point[] diagonals = new Point[] { new Point(1, 1), new Point(-1, 1), new Point(1, -1), new Point(-1, -1) };

    public static int GetDistance(RangeType rangeType, Point p1, Point p2)
    {
        switch (rangeType)
        {
            case RangeType.square:
                int x = Mathf.Abs(p1.x - p2.x);
                int y = Mathf.Abs(p1.y - p2.y);
                return Mathf.Max(x, y);

            case RangeType.circle:
                x = Mathf.Abs(p1.x - p2.x);
                y = Mathf.Abs(p1.y - p2.y);
                
                if (x == 0 || y == 0)
                {
                    return Mathf.Max(x, y);
                }

                return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y) - 1;

            default:
                return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y);
        }
    }

    public static List<Point> GetTiles(CombatMap map, RangeType rangeType, Point position, int maxRange, int minRange = 0)
    {
        List<Point> tiles = new List<Point>();

        // Gather valid tiles
        switch (rangeType)
        {
            case RangeType.distance:
                tiles = GetDistanceTiles(map, position, maxRange, minRange);
                break;

            case RangeType.circle:
                tiles = GetCircleTiles(map, position, maxRange, minRange);
                break;

            case RangeType.square:
                tiles = GetSquareTiles(map, position, maxRange, minRange);
                break;

            case RangeType.line:
                tiles = GetVectorTiles(map, position, lines, maxRange, minRange);
                break;

            case RangeType.diagonal:
                tiles = GetVectorTiles(map, position, diagonals, maxRange, minRange);
                break;
        }

        // Check valid targets

        return tiles;
    }

    public static List<Point> GetDistanceTiles(CombatMap map, Point position, int maxRange, int minRange = 0)
    {
        List<Point> tiles = new List<Point>();

        for (int x = -maxRange; x <= maxRange; x++)
        {
            for (int y = -maxRange; y <= maxRange; y++)
            {
                int distance = Mathf.Abs(x) + Mathf.Abs(y);
                if (distance < minRange || distance > maxRange)
                {
                    continue;
                }

                Point tile = new Point(x + position.x, y + position.y);
                if (map.IsWithinBounds(tile))
                {
                    tiles.Add(tile);
                }
            }
        }

        return tiles;
    }

    public static List<Point> GetSquareTiles(CombatMap map, Point position, int maxRange, int minRange = 0)
    {
        List<Point> tiles = new List<Point>();

        for (int x = -maxRange; x <= maxRange; x++)
        {

            for (int y = -maxRange; y <= maxRange; y++)
            {

                // Skip while inside minRange
                if (Mathf.Abs(x) + Mathf.Abs(y) < minRange)
                {
                    continue;
                }
                // Center on owner and check if tile is within bounds
                Point tile = position + new Point(x, y);
                if (map.IsWithinBounds(tile))
                {
                    tiles.Add(tile);
                }
            }
        }

        return tiles;
    }

    public static List<Point> GetCircleTiles(CombatMap map, Point position, int maxRange, int minRange = 0)
    {
        List<Point> tiles = new List<Point>();

        for (int x = -maxRange; x <= maxRange; x++)
        {
            for (int y = -maxRange; y <= maxRange; y++)
            {
                int distance = Mathf.Abs(x) + Mathf.Abs(y);
                if (distance < minRange || distance > (maxRange + 1))
                {
                    continue;
                }

                Point tile = new Point(x + position.x, y + position.y);
                if (map.IsWithinBounds(tile))
                {
                    tiles.Add(tile);
                }
            }
        }

        return tiles;
    }

    public static List<Point> GetVectorTiles(CombatMap map, Point position, Point[] vectors, int maxRange, int minRange = 0)
    {

        List<Point> tiles = new List<Point>();

        foreach (Point vector in vectors)
        {
            for (int i = minRange; i <= maxRange; i++)
            {
                Point tile = position + vector * i;
                if (map.IsWithinBounds(tile))
                {
                    tiles.Add(tile);
                }
            }
        }

        return tiles;
    }
}

public enum RangeType { distance, square, line, diagonal, circle }
public enum Blocking { nothing, enemies, allies, creatures, terrain, everything }
public enum Effects { friend, enemy, all }