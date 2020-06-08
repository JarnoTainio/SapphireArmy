using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Melee", menuName = "Sapphire/Melee")]
public class Melee : Action
{
    [Header("Melee")]
    public int damage = 1;
    public int maxDamage = 2;

    public override string GetDescription()
    {
        string str = "";
        if (damage < maxDamage)
        {
            str += damage + " - " + maxDamage +" damage";
        }
        else
        {
            str += damage + " damage";
        }
        return str;
    }

    public override void Perform(CombatMap map, Point trigger)
    {
        owner.Energy(-cost);
        UnitPiece target = map.unitManager.GetUnit(trigger);
        target.Damage(owner, Random.Range(damage, maxDamage + 1));
    }
}
