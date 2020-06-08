using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public CombatManager combatManager;
    public CombatMap map;
    public UnitManager unitManager;

    public void PerformTurn()
    {
        // Debug.Log("AI Move");
        // Collect units
        List<UnitPiece> army = unitManager.army;
        List<UnitPiece> monsters = unitManager.monsters;

        UnitPiece best = null;
        int bestDistance = int.MaxValue;
        Point bestMove = new Point();
        bool adjacent = false;

        foreach (UnitPiece monster in monsters)
        {
            if (!monster.unit.hasActionLeft)
            {
                continue;
            }

            foreach (UnitPiece soldier in army)
            {
                int d = Mathf.Abs(monster.unit.position.x - soldier.unit.position.x) + Mathf.Abs(monster.unit.position.y - soldier.unit.position.y);
                if (d == 1)
                {
                    best = monster;
                    adjacent = true;
                    bestDistance = 1;
                    break;
                }
            }
            if (adjacent)
            {
                break;
            }

            Point closest = new Point();
            int distance = int.MaxValue;
            
            foreach (Point point in monster.GetMoves(map))
            {
                foreach (UnitPiece soldier in army)
                {
                    int d = Mathf.Abs(point.x - soldier.unit.position.x) + Mathf.Abs(point.y - soldier.unit.position.y) + point.z;
                    if (d < distance)
                    {
                        distance = d;
                        closest = point;
                    }
                }
            }

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestMove = closest;
                best = monster;
            }
        }

        map.unitManager.SelectUnit(best.unit.position, Faction.Monster);
        if (!adjacent)
        {
            map.unitManager.WalkTo(bestMove, Faction.Monster);
        }
        else
        {
            PerformAction();
        }
    }

    public void PerformAction()
    {
        //Debug.Log("AI Action");
        // Collect units
        List<UnitPiece> army = unitManager.army;
        List<UnitPiece> monsters = unitManager.monsters;

        // Can best attack somebody?
        UnitPiece piece = unitManager.selectedUnit;
        Action[] actions = piece.unit.actions;
        int bestAction = -1;
        Point targetTile = new Point(-1,-1);
        for (int i = 0; i < actions.Length; i++)
        {
            if (actions[i] == null || !actions[i].CanUse())
            {
                continue;
            }

            List<TargetTile> targets = piece.unit.actions[i].GetTargets(map, false);

            if (targets.Count > 0)
            {
                // Do something
                if (bestAction == -1)
                {
                    bestAction = i;
                    targetTile = targets[Random.Range(0, targets.Count)].point;
                }

                // Prefer hostile actions
                else if (actions[i].IsHostile() && !actions[bestAction].IsHostile())
                {
                    bestAction = i;
                    targetTile = targets[Random.Range(0, targets.Count)].point;
                }

                // Prefer expensive actions
                else if (actions[i].IsHostile() == actions[bestAction].IsHostile() && actions[i].cost > actions[bestAction].cost)
                {
                    bestAction = i;
                    targetTile = targets[Random.Range(0, targets.Count)].point;
                }
            }

        }

        if (bestAction >= 0)
        {
            unitManager.SelectAction(bestAction);
            unitManager.PerformAction(targetTile);
            return;
        }

        combatManager.EndTurn();
    }
}
