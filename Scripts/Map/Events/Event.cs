using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event", menuName = "Sapphire/Event")]
public class Event : ScriptableObject
{
    [Header("Event")]
    public Sprite icon;
    public Rarity rarity;
    public EventType type;

    [TextArea]
    public string description;

    [Header("Button")]
    public string buttonText;

    [Header("Links")]
    public Event[] links;

    [Header("Effects")]
    public string hoverString;
    public EventEffect[] effects;

    [Header("Combats")]
    public Combat[] combats;

    public string GetHint()
    {
        string str = "";
        if (hoverString != null && hoverString.Length > 0)
        {
            str += hoverString +"\n";
        }
        if (combats.Length > 0)
        {
            str += "Combat\n";
        }
        for (int i = 0; i < effects.Length; i++)
        {
            if (i != 0)
            {
                str += ", ";
            }

            if (effects[i].value > 0)
            {
                str += "+";
            }

            str += effects[i].value + " " + effects[i].reward;

            if (effects[i].value < -1 || effects[i].value > 1)
            {
                str += "s";
            }
        }
        return str;
    }

    public int GetRarityWeight()
    {
        switch (rarity)
        {
            case Rarity.Common:
                return 60;

            case Rarity.Uncommon:
                return 25;

            case Rarity.Rare:
                return 10;

            case Rarity.VeryRare:
                return 4;

            case Rarity.UltraRare:
                return 1;
        }
        return 0;
    }
}

[System.Serializable]
public class EventEffect
{
    public Reward reward;
    public int value;

    public bool CanBeDone()
    {
        if (value >= 0)
        {
            return true;
        }

        switch (reward)
        {
            case (Reward.Resource):
                return GameData.instance.data.resources >= -value;

            case (Reward.MaxResource):
                return GameData.instance.data.maxResources >= -value;

            case (Reward.Coin):
                return GameData.instance.data.coins >= -value;
            
            default:
                return false;
        }

    }


}


public enum Reward { Resource, MaxResource, Coin}
public enum EventType { Combat, Event }

public enum Rarity { Common, Uncommon, Rare, VeryRare, UltraRare}
