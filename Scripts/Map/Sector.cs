using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sector", menuName = "Sapphire/Sector")]
public class Sector : ScriptableObject
{
    public Event[] events;
    public Event[] combats;

    [Header("Values")]
    public int brokenBridges;

    [Header("Weights")]
    public int eventWeight;
    public int combatWeight;
    public int emptyWeight;

    [Header("Names")]
    public string[] adjective;
    public string[] subjective;

    [HideInInspector]
    public MapObject[,] grid;
    [HideInInspector]
    public Point point;
    
    public string sectorName;

    public Event GetEvent(int seed, int x, int y)
    {
        Random.InitState(seed + x * y + seed * x + seed * y + (x + 1000) * seed + (y - 1000) * seed / 3);

        int total = eventWeight + combatWeight + emptyWeight;
        int roll = Random.Range(0, total);

        // Event
        roll -= eventWeight;
        if (roll < 0)
        {
            total = 0;
            foreach(Event e in events)
            {
                total += e.GetRarityWeight();
            }

            roll = Random.Range(0, total);

            foreach(Event e in events)
            {
                roll -= e.GetRarityWeight();
                if (roll < 0)
                {
                    return e;
                }
            }

            return null;
        }

        // Combat
        roll -= combatWeight;
        if (roll < 0)
        {
            return combats[Random.Range(0, combats.Length)];
        }

        // Empty
        return null;
    }

    public void GenerateName(int seed)
    {
        int rollSeed = (seed / 2 + 10000) * point.x + (seed / 3 - 10000) * point.y + point.x * point.y * seed;
        Random.InitState(rollSeed);
        sectorName = adjective[Random.Range(0, adjective.Length)] + " " + subjective[Random.Range(0, subjective.Length)];
    }

    public int GetDifficulty()
    {
        return Mathf.Abs(point.x) + Mathf.Abs(point.y);
    }

    public int GetCombatDifficulty()
    {
        int diff = GetDifficulty();
        return diff * 5 + Random.Range(4 + diff, 10 + diff * 2);
    }
}
