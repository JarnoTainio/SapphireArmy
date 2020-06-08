using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Effect", menuName = "Sapphire/Effect")]
public class Effect : Action
{
    [Header("Effect")]
    public Attribute attribute;
    public int value = 1;
    public int maxValue = 1;

    [Header("Target")]
    public bool all;

    public override string GetDescription()
    {
        string str = "";
        if (value < maxValue)
        {
            str +=  value + " - " + maxValue;
        }
        else
        {
            if (value > 0)
            {
                str += "+";
            }
            str += value;
        }

        str += " " + attribute.ToString();

        return str;
    }

    public override void Perform(CombatMap map, Point trigger)
    {
        owner.Energy(-cost);
        //owner.Armor(value);
        UnitPiece target = map.unitManager.GetUnit(trigger);
        if (target != null)
        {
            target.Effect(attribute, Random.Range(value, maxValue + 1));
        }
    }
}
