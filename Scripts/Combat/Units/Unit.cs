using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Sapphire/Unit")]
public class Unit : ScriptableObject
{
    public string unitName = "New unit";

    [Header("Art")]
    public Sprite sprite;
    public Animator animator;

    [Header("Sounds")]
    public AudioClip woundedSound;
    public AudioClip dyingSound;
    public AudioClip blockingSound;
    public AudioClip restingSound;
    public AudioClip[] walkingSounds;

    [Header("Attributes")]
    public int maxLife = 1;
    public int maxEnergy = 1;
    public int maxArmor = 1;
    public int move = 2;
    public int reviveCost = 1;
    public RangeType moveType;

    [Header("Actions")]
    public Action[] actions = new Action[3];

    [Header("Passives")]
    public Passive[] passives = new Passive[3];

    [Header("Other")]
    public Point position;
    public int difficulty;

    [HideInInspector]
    public int bonusMove = 0;
    [HideInInspector]
    public int life = 1;
    [HideInInspector]
    public int armor = 0;
    [HideInInspector]
    public int energy = 0;

    [HideInInspector]
    public Faction faction;
    [HideInInspector]
    public bool hasActionLeft;

    public void Init(bool first = false)
    {
        life = GetMaxLife();
        energy = 0;

        for(int i = 0; i < actions.Length; i++)
        {
            if (actions[i] != null)
            {
                actions[i] = Instantiate(actions[i]);
                actions[i].SetOwner(this);
            }
        }

        for (int i = 0; i < passives.Length; i++)
        {
            if (passives[i] != null)
            {
                passives[i] = Instantiate(passives[i]);
                if (!first)
                {
                    passives[i].SetOwner(this);
                }
                else
                {
                    EquipPassive(passives[i], i);
                }

            }
        }
    }



    /*==================================================================
    *  TIME
    * ================================================================*/

    public bool NewRound(CombatMap map, bool firstRound)
    {
        bool triggered = false;
        foreach(Passive passive in passives)
        {
            if (passive != null)
            {
                passive.Tick(firstRound ? Trigger.CombatStart : Trigger.NewRound);
                triggered = true;
            }
        }
        hasActionLeft = true;
        return triggered;
    }

    public void EndTurn()
    {
        hasActionLeft = false;
    }

    /*==================================================================
    *  MOVES
    * ================================================================*/

    public List<Point> GetMoves(CombatMap map)
    {
        int energyMove = energy > 0 ? 1 : 0;
        int baseRange = move + bonusMove;
        int energyRange = baseRange + energyMove;

        // Get valid moves
        List<Point> moves = PathFinding.GetTiles(map, moveType, position, energyRange, 1);
        List<Point> result = new List<Point>();
        foreach(Point p in moves)
        {
            if (map.unitManager.CanEnter(p))
            {
                result.Add(p);
            }
        }

        // Check energy tiles
        for(int i = 0; i < result.Count; i++)
        {
            Point p = result[i];
            int distance = PathFinding.GetDistance(moveType, position, p);
            p.z = (byte)(distance > baseRange ? 1 : 0);
            result[i] = p;

        }

        return result;
    }

    public void MoveTo(CombatMap map, Point tile)
    {
        position = tile;
    }

    public int GetMaxLife()
    {
        return Mathf.Max(1, maxLife);
    }

    public int GetMaxArmor()
    {
        return Mathf.Max(0, maxArmor);
    }

    public int GetMaxEnergy()
    {
        return Mathf.Max(0, maxEnergy);
    }

    public int GetReviveCost()
    {
        return Mathf.Max(0, reviveCost);
    }

    /*==================================================================
    *  COMBAT
    * ================================================================*/

    public int ModifyAttribute(Attribute attribute, int value)
    {
        switch (attribute)
        {
            case Attribute.Armor:
                return Armor(value);

            case Attribute.Energy:
                return Energy(value);

            case Attribute.Life:
                return Life(value);

            case Attribute.Move:
                bonusMove += value;
                return value;

            case Attribute.MaxLife:
                maxLife += value;
                return value;

            case Attribute.MaxArmor:
                maxArmor += value;
                return value;

            case Attribute.MaxEnergy:
                maxEnergy += value;
                return value;

            case Attribute.ReviveCost:
                reviveCost += value;
                return value;

            default:
                Debug.LogError("Unhandled attribute case: " + attribute);
                return 0;
        }
    }

    public int Life(int amount)
    {
        int modified = 0;
        if (amount > 0)
        {
            if (life + amount <= GetMaxLife())
            {
                modified = amount;
            }
            else
            {
                modified = GetMaxLife() - life;
            }
            life = Mathf.Min(life + amount, GetMaxLife());
        }
        else if (amount < 0)
        {
            modified = amount;
            life = Mathf.Max(life + amount, 0);
        }
        return modified;
    }

    public int Armor(int amount)
    {
        int modified = 0;
        if (amount > 0)
        {
            if (armor + amount <= GetMaxArmor())
            {
                modified = amount;
            }
            else
            {
                modified = GetMaxArmor() - armor;
            }
            armor = Mathf.Min(armor + amount, maxArmor);
        }
        else if (amount < 0)
        {
            modified = amount;
            armor = Mathf.Max(armor + amount, 0);
        }
        return modified;
    }

    public int Energy(int amount)
    {
        int modified = 0;
        if (amount > 0)
        {
            if (energy + amount <= GetMaxEnergy())
            {
                modified = amount;
            }
            else
            {
                modified = GetMaxEnergy() - energy;
            }
            energy = Mathf.Min(energy + amount, maxEnergy);
        }
        else if (amount < 0)
        {
            modified = amount;
            energy = Mathf.Max(energy + amount, 0);
        }
        return modified;
    }

    public int Damage(Unit attacker, int damage)
    {
        // Ignore slapping
        if (damage <= 0)
        {
            return 0;
        }

        // Block damage
        if (armor > 0)
        {
            armor -= damage;
            if (armor >= 0)
            {
                return 0;
            }
            damage = -armor;
            armor = 0;
        }

        // Suffer damage
        life -= damage;
        if (life <= 0)
        {
            life = 0;
            attacker.OnKill(this);
        }
        return damage;
    }

    /*==================================================================
    *  TRIGGERS
    * ================================================================*/

    public void OnKill(Unit victim)
    {
        foreach(Passive passive in passives)
        {
            if (passive != null)
            {
                passive.Tick(Trigger.OnKill);
            }
        }
    }

    /*==================================================================
    *  PASSIVES
    * ================================================================*/

    public void EquipPassive(Passive passive, int index = -1)
    {
        // Find first empty slot
        if (index == -1)
        {
            for(int i = 0; i < passives.Length; i++)
            {
                if (passives[i] == null)
                {
                    index = i;
                    break;
                }
            }
        }

        // Check that index is legal
        if (index == -1 || index >= passives.Length)
        {
            return;
        }

        //Remove old passive
        RemovePassive(index);

        // Set new passive
        passives[index] = passive;
        passive.owner = this;

        // Equipping trigger
        passive.Tick(Trigger.Equip);
    }

    public void RemovePassive(int index)
    {
        if (index < passives.Length     // Within bounds
            && passives[index] != null  // Something to remove
            && passives[index].owner != null)   // If owner is null, that means that this passive is placeholder
        {
            passives[index].Tick(Trigger.Unequip);
            passives[index] = null;
        }
    }

    /*==================================================================
    *  BOOLEAN HELPERS
    * ================================================================*/

    public bool IsFriendly(Unit unit)
    {
        return faction == unit.faction;
    }

    public bool IsEnemy(Unit unit)
    {
        return faction != unit.faction;
    }

    public bool IsAlive()
    {
        return life > 0;
    }

    /*==================================================================
    *  SAVE
    * ================================================================*/

    public void Load(string[] parts)
    {
        int index = 1;
        position = new Point(int.Parse(parts[index++]), int.Parse(parts[index++]));

        for (int i = 0; i < 3; i++)
        {
            if (parts[index] == "none")
            {
                actions[i] = null;
                index++;
            }
            else
            {
                actions[i] = Config.GetAction(parts[index++]);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (parts[index] == "none")
            {
                passives[i] = null;
                index++;
            }
            else
            {
                passives[i] = Config.GetPassive(parts[index++]);
            }
        }
        life = int.Parse(parts[index++]);
        maxLife = int.Parse(parts[index++]);
        energy = int.Parse(parts[index++]);
        maxEnergy = int.Parse(parts[index++]);
        armor = int.Parse(parts[index++]);
        maxArmor = int.Parse(parts[index++]);
        move = int.Parse(parts[index++]);
        bonusMove = int.Parse(parts[index++]);
        hasActionLeft = bool.Parse(parts[index++]);
    }

    public override string ToString()
    {
        string str = "";

        // Remove (clone) from name
        int index = name.IndexOf('(');
        if (index == -1)
        {
            str += name + " ";
        }
        else
        {
            str += name.Substring(0, index) + " ";
        }

        // Position
        str += position.x + " " + position.y + " ";

        // Actions
        for(int i = 0; i < actions.Length; i++)
        {
            if (actions[i] != null)
            {
                str += actions[i].ToString() + " ";
            }
            else
            {
                str += "none ";
            }
        }

        // Passives
        for (int i = 0; i < passives.Length; i++)
        {
            if (passives[i] != null)
            {
                str += passives[i].ToString() + " ";
            }
            else
            {
                str += "none ";
            }
        }

        // Attributes
        str += life + " " + maxLife + " ";
        str += energy + " " + maxEnergy + " ";
        str += armor + " " + maxArmor + " ";
        str += move + " " + bonusMove + " ";
        str += hasActionLeft;

        return str;
    }

    public static string AttributeToString(Attribute attribute)
    {
        switch (attribute)
        {
            case (Attribute.Life):
                return "Life";
            case Attribute.Armor:
                return "Armor";
            case (Attribute.Energy):
                return "Energy";
            case Attribute.Move:
                return "Move";
            case (Attribute.MaxLife):
                return "Life";
            case Attribute.MaxArmor:
                return "Armor";
            case (Attribute.MaxEnergy):
                return "Energy";
            case Attribute.ReviveCost:
                return "Revive Cost";
            default:
                return "???";
        }
        
    }
}
public enum Faction { Player, Monster, Neutral }
public enum Attribute { Life, Armor, Energy, Move, MaxLife, MaxArmor, MaxEnergy, ReviveCost }