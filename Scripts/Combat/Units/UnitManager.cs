using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [Header("References")]
    public CombatManager combatManager;
    public CombatMap mapManager;

    [Header("Units")]
    public UnitPiece unitPrefab;
    UnitPiece[,] unitGrid;
    public List<UnitPiece> army;
    public List<UnitPiece> monsters;
    List<UnitPiece> initiativeOrder;
    public Sprite[] unitSprites;

    // Selected unit
    public UnitPiece selectedUnit;
    public UnitPiece inspectedUnit;

    // Movement
    bool hasMoved;
    bool actionPerformed;
    Point startingTile;
    List<Point> moves;

    public List<int> fallenUnits;

    // Action
    Action action;
    List<TargetTile> targets;
    List<Point> hoverInfo;
    public bool isActive;

    private void Awake()
    {
        army = new List<UnitPiece>();
        monsters = new List<UnitPiece>();
        targets = new List<TargetTile>();
        initiativeOrder = new List<UnitPiece>();
        hoverInfo = new List<Point>();
        moves = new List<Point>();
        fallenUnits = new List<int>();
    }

    public void Load(CombatMap map, string data, string fallen)
    {
        string[] armies = data.Split(':');
        string[] armyData = armies[0].Split(',');
        string[] monsterData = armies[1].Split(',');

        mapManager = map;
        unitGrid = new UnitPiece[map.width, map.height];

        for (int i = 0; i < armyData.Length; i++)
        {
            string[] parts = armyData[i].Split();
            Unit baseUnit = Config.GetUnit(parts[0]);
            Unit unit = Instantiate(baseUnit);
            unit.name = baseUnit.name;
            unit.Load(parts);
            AddUnit(new Point(unit.position.x, unit.position.y), unit, Faction.Player);
        }

        for (int i = 0; i < monsterData.Length; i++)
        {
            if (monsterData[i].Length < 2)
            {
                continue;
            }
            string[] parts = monsterData[i].Split();
            Unit baseUnit = Config.GetUnit(parts[0]);
            Unit unit = Instantiate(baseUnit);
            unit.name = baseUnit.name;
            unit.Load(parts);
            AddUnit(new Point(unit.position.x, unit.position.y), unit, Faction.Monster);
        }
        if (fallen.Length > 0)
        {
            foreach (string s in fallen.Split())
            {
                fallenUnits.Add(int.Parse(s));
            }
        }
    }

    public void Load(CombatMap map, Unit[] army, Unit[] monsters)
    {
        mapManager = map;
        unitGrid = new UnitPiece[map.width, map.height];

        for (int i = 0; i < army.Length; i++)
        {
            Unit unit = Instantiate(army[i]);
            Point pos = new Point(0, map.height / 2 - 3) + unit.position;
            AddUnit(pos, unit, Faction.Player, i);
        }

        // Create monster deploy area
        // Point monsterOffset = new Point(map.width - 1, map.height / 2 - monsters.Length / 2);
        List<Point> startingPoints = new List<Point>();
        for(int x = map.width - 4; x < map.width; x++)
        {
            for (int y = 1; y < map.height - 1; y++)
            {
                startingPoints.Add(new Point(x, y));
            }
        }

        // Create monsters
        for (int i = 0; i < monsters.Length; i++)
        {
            Unit unit = Instantiate(monsters[i]);
            Point p = startingPoints[Random.Range(0, startingPoints.Count)];
            startingPoints.Remove(p);
            // AddUnit(new Point(monsterOffset.x, monsterOffset.y + i), unit, Faction.Monster);
            AddUnit(p, unit, Faction.Monster);
        }
    }


    /*==================================================================
    *  UNITS
    * ================================================================*/

    public void AddUnit(Point position, Unit unitData, Faction faction, int identifier = -1)
    {
        UnitPiece unit = Instantiate(unitPrefab, transform);
        unit.identifier = identifier;
        unit.SetUnit(unitData);
        unit.unit.faction = faction;
        unitGrid[position.x, position.y] = unit;

        if (faction == Faction.Player)
        {
            army.Add(unit);
        }
        else
        {
            unit.spriteRenderer.flipX = true;
            monsters.Add(unit);
        }
        initiativeOrder.Add(unit);
        unit.MoveTo(mapManager, position, -1);
    }

    public void RemoveUnit(Point position)
    {
        UnitPiece unit = unitGrid[position.x, position.y];
        initiativeOrder.Remove(unit);
        if (unit.unit.faction == Faction.Player)
        {
            fallenUnits.Add(unit.identifier);
            army.Remove(unit);
        }
        else
        {
            monsters.Remove(unit);
        }
        unitGrid[position.x, position.y] = null;
        unit.Die();

        // Player has lost all units
        if (army.Count == 0)
        {
            StartCoroutine(combatManager.EndCombat(false));
        }

        // No enemies left
        else if (monsters.Count == 0)
        {
            StartCoroutine(combatManager.EndCombat(true));
        }
    }

    public UnitPiece GetUnit(Point tile)
    {
        if (mapManager.IsWithinBounds(tile))
        {
            return unitGrid[tile.x, tile.y];
        }
        return null;
    }

    public void SelectUnit(Point tile, Faction faction)
    {
        UnitPiece unit = unitGrid[tile.x, tile.y];

        // Unit clicked
        if (unit != null && unit.unit.hasActionLeft)
        {
            // Friendly clicked
            if (unit.unit.faction == faction && !hasMoved)
            {
                selectedUnit = unit;
                startingTile = tile;
                hasMoved = false;
                actionPerformed = false;
                ShowMovement();
                combatManager.UnitSelected(selectedUnit.unit);
            }
        }
    }

    public void Unselect()
    {
        if (selectedUnit != null)
        {
            if (targets.Count > 0 && !hasMoved)
            {
                ShowMovement();
            }
            else if (hasMoved)
            {
                UndoMovement();
            }
            else
            {
                UnselectUnit();
            }
        }
    }

    public int ActiveUnits(Faction faction)
    {
        int count = 0;
        foreach(UnitPiece piece in faction == Faction.Player ? army : monsters)
        {
            if (piece.unit.hasActionLeft)
            {
                count++;
            }
        }
        return count;
    }

    public void EndTurn()
    {
        if (selectedUnit != null)
        {
            initiativeOrder.Remove(selectedUnit);
            initiativeOrder.Add(selectedUnit);
            selectedUnit.EndTurn();
            if (!actionPerformed)
            {
                selectedUnit.Effect(Attribute.Energy, 1);
            }
            selectedUnit = null;
        }
        hasMoved = false;
        actionPerformed = false;
    }

    /*==================================================================
    *  ACTION
    * ================================================================*/

    public void ShowAction(int index)
    {
        if (!SelectAction(index))
        {
            return;
        }

        mapManager.Mark(selectedUnit.unit.position, Marking.Orange);

        Marking mark = 0;
        if (action.type == ActionType.Attack)
        {
            mark = Marking.Red;
        }
        else if (action.type == ActionType.Buff)
        {
            mark = Marking.Purple;
        }

        mapManager.RemoveMarkings();
        foreach(TargetTile targetTile in targets)
        {
            Marking m = mark;
            if (targetTile.type == TargetType.NotValid)
            {
                m = Marking.Orange;
            }
            mapManager.Mark(targetTile.point, m);
        }
    }

    public void Preview(Action action)
    {
        mapManager.RemoveMarkings();
        moves.Clear();
        targets.Clear();
        foreach(TargetTile tt in action.GetTargets(mapManager))
        {
            mapManager.Mark(tt.point, Marking.Orange);
        }
    }

    public void ShowCurrent()
    {
        mapManager.RemoveMarkings();
        if (selectedUnit != null)
        {
            if (!hasMoved)
            {
                ShowMovement();
            }
            else
            {
                ShowAction(0);
            }
        }
    }

    public bool SelectAction(int index)
    {
        if (selectedUnit.unit.actions[index] == null)
        {
            return false;
        }

        targets.Clear();
        if (selectedUnit.unit.actions[index].CanUse())
        {
            action = selectedUnit.unit.actions[index];
            targets = action.GetTargets(mapManager);
            return true;
        }
        else
        {
            Preview(selectedUnit.unit.actions[index]);
            return false;
        }
    }

    public void PerformAction(UnitPiece piece, Action action, Point tile, bool free = false)
    {
        if (!free)
        {
            piece.unit.hasActionLeft = false;
        }
        action.Perform(mapManager, tile);
        piece.PerformingAction(action);
        piece.Animation(action.type);
        if (action.type == ActionType.Attack)
        {
            Unit target = GetUnit(tile).unit;
            if (!target.IsAlive())
            {
                RemoveUnit(tile);
            }
        }
    }

    public bool PerformAction(Point tile)
    {
        bool found = false;
        foreach(TargetTile targetTile in targets)
        {
            if (targetTile.point == tile)
            {
                found = targetTile.type == TargetType.Valid;
                break;
            }
        }

        if (found)
        {
            if (!action.CanUse())
            {
                // ToDo: Show lack of energy
                return false;
            }
            actionPerformed = true;
            targets.Clear();
            mapManager.RemoveMarkings();
            action.Perform(mapManager, tile);
            selectedUnit.PerformingAction(action);
            selectedUnit.Animation(action.type);
            if (action.type == ActionType.Attack)
            {
                Unit target = GetUnit(tile).unit;
                if (!target.IsAlive())
                {
                    RemoveUnit(tile);
                }
            }
            combatManager.EndTurn();
            return true;
        }
        return false;
    }

    public void UnselectAction()
    {
        targets.Clear();
        ShowMovement();
    }

    public void UnselectUnit()
    {
        selectedUnit = null;
        mapManager.RemoveMarkings();
        combatManager.UnitUnselected();
    }


    /*==================================================================
    *  MOVEMENT
    * ================================================================*/

    public void ShowMovement()
    {
        mapManager.RemoveMarkings();
        mapManager.Mark(selectedUnit.unit.position, Marking.Orange);
        moves = selectedUnit.GetMoves(mapManager);

        if (hasMoved)
        {
            return;
        }
        foreach (Point move in moves)
        {
            mapManager.Mark(move, move.z == 0 ? Marking.Green : Marking.Energy);
        }
    }

    public void WalkTo(Point tile, Faction faction, float speed = 3f)
    {
        if (PerformAction(tile)){
            return;
        }

        if (hasMoved)
        {
            return;
        }
        UnitPiece unit = unitGrid[tile.x, tile.y];

        // Already selected unit clicked
        if (selectedUnit == unit)
        {
            UnselectUnit();
        }

        else if (unit == null)
        {
            foreach (Point move in moves)
            {
                if (move == tile)
                {
                    hasMoved = true;
                    Move(selectedUnit, tile);
                    selectedUnit.MoveTo(mapManager, move, speed);
                    mapManager.RemoveMarkings();
                    combatManager.MoveDone();
                }
            }
        }

        else if (unit.unit.faction == faction)
        {
            SelectUnit(tile, faction);
        }
    }

    public void Move(UnitPiece unit, Point tile)
    {
        unitGrid[unit.unit.position.x, unit.unit.position.y] = null;
        unitGrid[tile.x, tile.y] = unit;
    }

    public void UndoMovement()
    {
        selectedUnit.UndoMove();
        Move(selectedUnit, startingTile);
        selectedUnit.MoveTo(mapManager, startingTile, 16f);
        hasMoved = false;
        ShowMovement();
    }

    public bool CanEnter(Point point)
    {
        if (mapManager.IsWithinBounds(point) && GetUnit(point) == null)
        {
            return true;
        }
        return false;
    }

    /*==================================================================
    *  MOUSE HOVERING
    * ================================================================*/

    public void TileHover(Point tile)
    {
        if (selectedUnit != null)
        {
            return;
        }

        Unit unit = GetUnit(tile)?.unit;

        if (unit != null)
        {
            mapManager.RemoveMarkings(moves);

            hoverInfo = unit.GetMoves(mapManager);
            mapManager.Mark(tile, unit.hasActionLeft ? Marking.Orange : Marking.Red);

            Marking normalMove = unit.hasActionLeft ? Marking.Green : Marking.Orange;
            Marking energyMove = unit.hasActionLeft ? Marking.Energy : Marking.Red;

            foreach (Point p in hoverInfo) {
                mapManager.Mark(p, p.z == 0 ? normalMove : energyMove);
            }

        }
        else if (hoverInfo.Count > 0)
        {
            mapManager.RemoveMarkings(hoverInfo);
            hoverInfo.Clear();
            
        }
    }

    public void TileHoverEnd(Point tile)
    {
        if (selectedUnit != null)
        {
            return;
        }

        if (hoverInfo.Count > 0)
        {
            hoverInfo.Add(tile);
            mapManager.RemoveMarkings(hoverInfo);
            hoverInfo.Clear();
        }
        if (selectedUnit != null)
        {
            if (!hasMoved)
            {
                ShowMovement();
            }
        }
    }

    /*==================================================================
    *  ROUNDS
    * ================================================================*/

    public IEnumerator NewRound(bool firstRound = false)
    {
        isActive = true;

        int index = 0;
        while (index < initiativeOrder.Count)
        {
            UnitPiece piece = initiativeOrder[index];
            StartCoroutine(piece.NewRound(mapManager, firstRound));

            while (piece.isActive)
            {
                yield return null;
            }
            index++;
        }
        isActive = false;
    }



    /*==================================================================
    *  SAVE
    * ================================================================*/

    public override string ToString()
    {
        string str = "";

        // Player army
        for(int i = 0; i < army.Count; i++)
        {
            if (i != 0)
            {
                str += ",";
            }
            str += army[i].unit.ToString();
        }

        // Separate armies
        str += ":";

        // Monsters
        for (int i = 0; i < monsters.Count; i++)
        {
            if (i != 0)
            {
                str += ",";
            }
            str += monsters[i].unit.ToString();
        }

        str += "\n";

        for(int i = 0; i < fallenUnits.Count; i++)
        {
            if (i != 0)
            {
                str += " ";
            }
            str += fallenUnits[i];
        }
        str += "\n";

        return str;
    }

}
