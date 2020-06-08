using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public abstract class Action : ScriptableObject
{
    public string actionName = "New action";
    public Sprite icon;
    public ActionType type;

    [Header("Sound")]
    public AudioClip sound;

    [Header("Energy")]
    public int cost = 0;

    [Header("Targeting")]
    public Target canTarget;
    public Effects affects;

    [Header("Range")]
    public int minRange = 0;
    public int maxRange = 0;
    public Blocking blocking;
    public RangeType rangeType = RangeType.distance;

    [HideInInspector]
    public string description;
    public Unit owner;

    public void SetOwner(Unit owner)
    {
        this.owner = owner;
    }

    public List<TargetTile> GetTargets(CombatMap map, bool allTiles = true)
    {
        // Get all tiles
        List<Point> tiles = PathFinding.GetTiles(map, rangeType, owner.position, maxRange, minRange);
        
        // Filter to valid targets
        List<TargetTile> targetTiles = new List<TargetTile>();
        foreach(Point point in tiles)
        {
            if (allTiles || IsValidTarget(map, point) == TargetType.Valid)
            {
                targetTiles.Add(new TargetTile(point, IsValidTarget(map, point)));
            }
        }

        return targetTiles;
    }

    TargetType IsValidTarget(CombatMap map, Point point)
    {
        switch (canTarget) {
            case Target.anything:
                return TargetType.Valid;

            case Target.creature:
                if (map.unitManager.GetUnit(point) != null)
                {
                    return TargetType.Valid;
                }
                return TargetType.NotValid;

            case Target.enemy:
                UnitPiece unit = map.unitManager.GetUnit(point);
                if (unit != null && unit.unit.IsEnemy(owner))
                {
                    return TargetType.Valid;
                }
                return TargetType.NotValid;

            case Target.ally:
                unit = map.unitManager.GetUnit(point);
                if (unit != null && unit.unit.IsFriendly(owner))
                {
                    return TargetType.Valid;
                }
                return TargetType.NotValid;
        }

        return TargetType.NotValid;
    }

    public bool CanUse()
    {
        return cost <= owner.energy;
    }

    public bool IsHostile()
    {
        if (type == ActionType.Attack)
        {
            return true;
        }
        if (type == ActionType.Debuff)
        {
            return true;
        }
        return false;
    }

    public abstract void Perform(CombatMap map, Point trigger);

    public abstract string GetDescription();

    public override string ToString()
    {
        int index = name.IndexOf('(');
        if (index == -1)
        {
            return name;;
        }
        else
        {
            return name.Substring(0, index);
        }
    }
}
public enum ActionType { Attack, Buff, Debuff }

public enum Target { ally, enemy, creature, anything, wall }

