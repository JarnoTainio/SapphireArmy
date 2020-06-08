using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
    public static int size;
    static MapObject[,] grid;
    static int[,] weights;

    static int islands;

    static Point[] directions = new Point[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) };
    
    public static int GateSeed(int seed, int x, int y)
    {
        int gateSeed = seed * x + seed * y + x * y;
        return gateSeed;
    }

    public static MapObject[,] CreateMap(int seed, Sector sector, int sectorX, int sectorY)
    {
        grid = new MapObject[size, size];

        // Create exit bridges
        List <Point> gates = new List<Point>();
        int center = (size - 1) / 2;
        int spawnArea = center / 2 - 2;

        // West
        UnityEngine.Random.InitState(GateSeed(seed, 10 * sectorX - 5, sectorY));
        gates.Add(new Point(0, center + UnityEngine.Random.Range(-spawnArea, spawnArea) * 2));

        // East
        UnityEngine.Random.InitState(GateSeed(seed, 10 * sectorX + 5, sectorY));
        gates.Add(new Point(size - 1, center + UnityEngine.Random.Range(-spawnArea, spawnArea) * 2));

        // South
        UnityEngine.Random.InitState(GateSeed(seed, sectorX, 10 * sectorY - 5));
        gates.Add(new Point(center + UnityEngine.Random.Range(-spawnArea, spawnArea) * 2, 0));

        // North
        UnityEngine.Random.InitState(GateSeed(seed, sectorX, 10 * sectorY + 5));
        gates.Add(new Point(center + UnityEngine.Random.Range(-spawnArea, spawnArea) * 2, size - 1));

        foreach(Point p in gates)
        {
            grid[p.x, p.y] = MapObject.bridge;
        }
        islands = 0;

        UnityEngine.Random.InitState(seed + sectorX * seed / 2 + sectorX * sectorY + sectorY * seed / 3);
        CreateWeights();

        // Build path from edge to edge
        BuildPath(sector, gates[0].x + 1, gates[0].y, gates[1].x - 1, gates[1].y);
        BuildPath(sector, gates[2].x, gates[2].y + 1, gates[3].x, gates[3].y - 1);
        BuildPath(sector, gates[3].x, gates[3].y - 1, gates[2].x, gates[2].y + 1);

        // Add random islands
        int islandTarget = (size / 2) * (size / 2) + UnityEngine.Random.Range(0, size * 2);
        while (islands < islandTarget)
        {
            Point p = new Point(UnityEngine.Random.Range(0, size / 2), UnityEngine.Random.Range(0, size / 2));
            if ((p.x == 0 || p.y == 0 || p.x == size / 2 || p.y == size / 2) && UnityEngine.Random.Range(0,1) == 0)
            {
                p = new Point(UnityEngine.Random.Range(0, size / 2), UnityEngine.Random.Range(0, size / 2));
            }
            p = new Point(p.x * 2 + 1, p.y * 2 + 1);
            if (grid[p.x, p.y] == MapObject.none)
            {

                BuildPath(sector, p.x, p.y, size - p.x +1, size - p.y + 1);
            }
            islands++;
        }

        for(int x = 1; x < size; x += 2)
        {
            for (int y = 1; y < size; y += 2)
            {
                if (grid[x, y] == MapObject.island)
                {
                    foreach (Point d in directions)
                    {
                        Point p2 = new Point(x + d.x * 2, y + d.y * 2);
                        if (p2.x > 0 && p2.x < size && p2.y > 0 && p2.y < size)
                        {
                            if (grid[p2.x, p2.y] == MapObject.island && UnityEngine.Random.Range(0, 3) == 0)
                            {
                                grid[x + d.x, y + d.y] = UnityEngine.Random.Range(0, 100) < sector.brokenBridges ? MapObject.brokenBridge : MapObject.bridge;
                            }
                        }
                    }
                }
            }
        }

        return grid;
    }

    private static void CreateWeights()
    {
        weights = new int[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                weights[x, y] = UnityEngine.Random.Range(1, 10) + UnityEngine.Random.Range(1, 10);
            }
        }
    }

    public static void BuildPath(Sector sector, int startX, int startY, int targetX, int targetY)
    {
        //Debug.Log("Build path from " + startX + ", " + startY + " to " + targetX + ", " + targetY);
        Node[,] nodeGrid = new Node[size, size];
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++)
            {
                nodeGrid[x, y] = new Node(x, y);
            }
        }
        List<Node> visited = new List<Node>();
        List<Node> open = new List<Node>();

        open.Add(nodeGrid[startX, startY]);
        Node current = null;

        while (open.Count > 0)
        {
            open.Sort((a, b) => a.cost.CompareTo(b.cost));
            current = open[0];
            if (grid[current.x, current.y] != 0)
            {
                break;
            }
            //Print(current, targetX, targetY);

            // Target reached
            if (current.x == targetX && current.y == targetY)
                break;

            // Remove from open and add to visited
            open.Remove(current);
            visited.Add(current);

            // Adjacent nodes
            foreach(Point p in directions)
            { 
                // Out of bounds checking
                int nx = current.x + p.x * 2;
                if (nx < 0 || nx >= size)
                {
                    continue;
                }
                int ny = current.y + p.y * 2;
                if (ny < 0 || ny >= size)
                {
                    continue;
                }

                // Legal node
                Node n = nodeGrid[nx, ny];
                int newCost = current.cost + weights[n.x, n.y];

                if (!visited.Contains(n) || n.cost > newCost)
                {
                    if (visited.Contains(n))
                    {
                        visited.Remove(n);
                    }
                    open.Add(n);

                    n.parent = current;
                    n.cost = newCost;
                }
            }
        }

        if (
            current != null && // Have current node
             (
                (current.x == targetX && current.y == targetY) // Node is target
             ||                                                // OR
                grid[current.x, current.y] != 0                // Node is island (collission)
             )
            )
        {
            while (current != null)
            {
                grid[current.x, current.y] = MapObject.island;
                islands++;
                if (current.parent != null)
                {
                    Point offSet = new Point((current.x - current.parent.x) / 2, (current.y - current.parent.y) / 2);
                    grid[current.x - offSet.x, current.y - offSet.y] = UnityEngine.Random.Range(0, 100) < sector.brokenBridges ? MapObject.brokenBridge : MapObject.bridge;
                }
                current = current.parent;
            }
        }
        else
        {
            throw new Exception("Failed to build map path");
        }
    }

    public static void Print(Node node, int targetX, int targetY)
    {
        string str = "";
        bool[,] temp = new bool[size, size];
        Node n = node;
        while (n != null)
        {
            temp[n.x, n.y] = true;
            n = n.parent;
        }

        for(int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (x == targetX && y == targetY)
                {
                    str += "+";
                }
                else
                {
                    str += temp[x, y] ? "o" : "*";
                }
            }
            str += "\n";
        }
        Debug.Log(str);
    }
}

public enum MapObject { none, bridge, brokenBridge, island, gate}

public class Node 
{
    public Node parent;
    public int x;
    public int y;
    public int cost;

    public Node(int x, int y, Node parent = null)
    {
        this.x = x;
        this.y = y;
        this.parent = parent;
    }
}



