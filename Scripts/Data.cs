using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
    public Point mapPosition = new Point(0, 0);

    // Profile data
    public int experience = 50;
    public int level = 1;

    // Run data
    public string scene = "MapScene";
    public int seed;
    public int day;

    // Resources
    public int maxCrystals = 3;
    public int resources = 10;
    public int maxResources = 10;

    public int coins = 0;

    // Visited islands
    public List<Point> visitedIslands = new List<Point>();

    // Event
    public string eventName = "none";

    // Army
    public List<Unit> army = new List<Unit>();

    public string combat = "";

    public void NewGame()
    {
        scene = "MapScene";

        day = 1;
        seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        mapPosition = new Point(-1, -1);

        maxResources = resources = 5 + level;
        coins = 0;

        visitedIslands = new List<Point>();
        eventName = "_NewGame";

        combat = "";
        maxCrystals = 3;

        // Reset army
        army.Clear();
        AddUnit("Army_Soldier", new Point(1, 1));
        AddUnit("Army_Soldier", new Point(1, 2));
        AddUnit("Army_Soldier", new Point(1, 3));
        AddUnit("Army_Archer", new Point(0, 2));
        
        LevelEffects();
    }

    private void LevelEffects()
    {

        if (level > 1)
        {
            AddUnit("Army_Guard", new Point(0, 1));
        }
        if (level > 2)
        {
            maxCrystals++;
        }
        if (level > 3)
        {
            AddUnit("Army_Spear", new Point(0, 4));
        }
        if (level > 5)
        {
            AddUnit("Army_Soldier", new Point(1, 4));
        }
        if (level > 7)
        {
            AddUnit("Army_Archer", new Point(0, 2));
        }
    }

    private void AddUnit(string unitName, Point position)
    {
        Unit unit = ScriptableObject.Instantiate(Config.GetUnit(unitName));
        unit.position = position;
        unit.Init(true);
        army.Add(unit);
    }

    public void RemoveFallen(List<int> fallenUnits)
    {
        List<Unit> fallen = new List<Unit>();
        foreach(int f in fallenUnits)
        {
            fallen.Add(army[f]);
        }
        foreach(Unit u in fallen)
        {
            army.Remove(u);
        }
    }

    public override string ToString()
    {
        string str = "";

        // PROFILE DATA
        str += level + " " + experience;
        str += "\n";

        // PLAYER DATA
        str += seed + " " + scene;
        str += " " + mapPosition.x + " " + mapPosition.y;

        // Resources
        str += " " + maxCrystals + " " + resources + " " + maxResources;
        str += " " + coins;
        str += "\n";

        // PLAYER UNITS
        for (int i = 0; i < army.Count; i++)
        {
            if (i != 0)
            {
                str += ",";
            }
            str += army[i].ToString();
        }
        str += "\n";

        // VISITED ISLANDS
        for (int i = 0; i < visitedIslands.Count; i++)
        {
            if (i != 0)
            {
                str += " ";
            }
            str += visitedIslands[i].x + " " + visitedIslands[i].y;
        }
        str += "\n";

        // CURRENT EVENT
        str += eventName;
        str += "\n";

        // COMBAT
        str += combat;
        str += "\n";

        return str;
    }

    public bool Load(string str)
    {
        try
        {
            string[] lines = str.Split('\n');
            int lineNumber = 0;
            int i = 0;

            // PROFILE DATA
            string[] parts = lines[lineNumber++].Split();
            level = int.Parse(parts[i++]);
            experience = int.Parse(parts[i++]);

            // PLAYER DATA
            parts = lines[lineNumber++].Split();
            i = 0;
            seed = int.Parse(parts[i++]);
            scene = parts[i++];

            mapPosition = new Point(int.Parse(parts[i++]), int.Parse(parts[i++]));

            maxCrystals = int.Parse(parts[i++]);
            resources = int.Parse(parts[i++]);
            maxResources = int.Parse(parts[i++]);
            coins = int.Parse(parts[i++]);

            // PLAYER UNITS
            parts = lines[lineNumber++].Split(',');
            army.Clear();
            foreach (string unitData in parts)
            {
                string[] pieces = unitData.Split();
                Unit baseUnit = Config.GetUnit(pieces[0]);
                Unit unit = ScriptableObject.Instantiate(baseUnit);
                unit.name = baseUnit.name;
                unit.Load(pieces);
                army.Add(unit);
            }

            // VISITED ISLANDS
            parts = lines[lineNumber++].Split();
            i = 0;

            visitedIslands = new List<Point>();
            while (parts.Length > 1 && i < parts.Length)
            {
                int x = int.Parse(parts[i++]);
                int y = int.Parse(parts[i++]);
                visitedIslands.Add(new Point(x, y));
            }

            // CURRENT EVENT
            parts = lines[lineNumber++].Split();
            i = 0;
            eventName = parts[i++];

            // COMBAT
            // Round, crystals
            if (lineNumber == lines.Length)
            {
                return true;
            }
            combat = lines[lineNumber++] + "\n";

            // Map, tiles
            if (lineNumber == lines.Length)
            {
                return true;
            }
            combat += lines[lineNumber++] + "\n";

            // Units
            if (lineNumber == lines.Length)
            {
                return true;
            }
            combat += lines[lineNumber++] + "\n";

            // Fallen units
            if (lineNumber == lines.Length)
            {
                return true;
            }
            combat += lines[lineNumber++] + "\n";

            
        }
        catch(Exception e)
        {
            Debug.Log(e);
            return false;
        }
        return true;
    }

    public string LevelReward(int level)
    {
        if (level == 2)
        {
            return "New unit: Guard!";
        }
        else if (level == 3)
        {
            return "+1 Command Crystal!";
        }
        else if (level == 4)
        {
            return "New unit: Lancer!";
        }
        else if (level == 6)
        {
            return "Extra Soldier!";
        }
        if (level == 8)
        {
            return "Extra Archer!";
        }
        return "+1 Maximum Resources!";
    }
}