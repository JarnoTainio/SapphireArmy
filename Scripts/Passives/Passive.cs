using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "Passive", menuName = "Sapphire/Passive")]
public class Passive : ScriptableObject
{
    public string passiveName;
    public Sprite icon;
    public string description;
    public PassiveEffect[] effects;

    [HideInInspector]
    public Unit owner;
    [HideInInspector]
    public UnitPiece ownerPiece;

    public void SetOwner(Unit owner)
    {
        this.owner = owner;
    }

    public virtual void Tick(Trigger trigger)
    {
        foreach(PassiveEffect pe in effects)
        {
            if (trigger == pe.trigger)
            {
                if (ownerPiece != null)
                {
                    Debug.Log(passiveName + " pieceTick " + trigger + " to " + owner.unitName);
                    ownerPiece.Effect(pe.attribute, pe.value);
                }
                else
                {
                    owner.ModifyAttribute(pe.attribute, pe.value);
                }
            }
        }
    }

    public string GetDescription()
    {
        if (description != null && description.Length > 0)
        {
            return description;
        }

        string str = "";
        foreach(PassiveEffect pe in effects)
        {
            string s = pe.ToString();
            if (s.Length > 0)
            {
                str += pe.ToString() + "\n";
            }
        }
        return str;
    }

    public override string ToString()
    {
        int index = name.IndexOf('(');
        if (index == -1)
        {
            return name; ;
        }
        else
        {
            return name.Substring(0, index);
        }
    }

}
public enum Trigger{Equip, Unequip, OnKill, NewRound, CombatStart }

[System.Serializable]
public class PassiveEffect
{
    public Trigger trigger;
    public Attribute attribute;
    public int value;

    public override string ToString()
    {
        switch (trigger)
        {
            case (Trigger.Equip):
                return AttributeToString();

            case Trigger.Unequip:
                return "";

            case Trigger.OnKill:
                return "On kill: " + AttributeToString();

            case Trigger.NewRound:
                return "New Round: " + AttributeToString();

            case Trigger.CombatStart:
                return "Start of Combat: " + AttributeToString();

            default:
                return "???";
        }
    }

    public string AttributeToString()
    {
        return (value > 0 ? "+" + value : "" + value) + " " + Unit.AttributeToString(attribute);
    }
}