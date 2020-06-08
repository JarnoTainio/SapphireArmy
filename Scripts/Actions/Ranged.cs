using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Ranged", menuName = "Sapphire/Ranged")]
public class Ranged : Action
{
    [Header("Ranged")]
    public GameObject projectilePrefab;
    public int damage = 1;
    public int maxDamage = 2;

    public override string GetDescription()
    {
        string str = "";
        if (damage < maxDamage)
        {
            str += damage + " - " + maxDamage + " damage";
        }
        else
        {
            str += damage + " damage";
        }
        str += "\nRange: " + minRange + " - " + maxRange;
        return str;
    }

    public override void Perform(CombatMap map, Point trigger)
    {
        owner.Energy(-cost);
        UnitPiece target = map.unitManager.GetUnit(trigger);
        target.Damage(owner, Random.Range(damage, maxDamage + 1));
    }
}
