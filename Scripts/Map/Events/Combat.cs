using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Combat", menuName = "Sapphire/Combat")]
public class Combat : ScriptableObject
{
    public int difficultyLevel;
    public int monsterCrystals;

    [Header("Map")]
    public int width;
    public int height;

    [Header("Type")]
    public CombatType[] tags;

    [Header("Monsters")]
    public Unit[] monsters;
    public Unit[] reinforcements;

    public Combat CreateInstance(int difficulty)
    {
        Combat combat = Instantiate(this);
        int danger = difficulty - difficultyLevel;

        List<Unit> attackers = new List<Unit>(monsters);
        attackers.Sort((a, b) => -a.difficulty.CompareTo(b.difficulty));

        foreach(Unit u in attackers)
        {
            danger -= u.difficulty;
        }

        // Make combat easier
        if (danger < 0)
        {
            if (combat.monsterCrystals > 1 && combat.monsterCrystals + Random.Range(0,2) > attackers.Count)
            {
                combat.monsterCrystals--;
            }
            else if (attackers.Count > 1)
            {
                attackers.Remove(attackers[0]);
            }
        }

        // Make combat more difficult
        else
        {

            int i = 10;
            while (danger > 0)
            {
                if (combat.monsterCrystals < 4 && combat.monsterCrystals <= attackers.Count - combat.monsterCrystals)
                {
                    combat.monsterCrystals++;
                    danger -= 8;
                }
                else if (attackers.Count > 1)
                {
                    Unit newUnit = reinforcements[Random.Range(0, reinforcements.Length)];
                    danger -= newUnit.difficulty;
                    attackers.Add(newUnit);
                }
                else
                {
                    break;
                }
                if (--i <= 0)
                {
                    break;
                }
            }
        }
        combat.monsters = attackers.ToArray();
        return combat;
    }
}
public enum CombatType { greenskin, undead, demon, forest, mountain }