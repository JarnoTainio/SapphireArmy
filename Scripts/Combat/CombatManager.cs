using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [Header("References")]
    public CombatMap mapManager;
    public UnitManager unitManager;
    public GraveyardManager graveyard;
    public CombatUI combatUI;
    public AI ai;

    [Header("Army stats")]
    public int armyCrystalMax;
    public int armyCrystalsLeft;

    public int monsterCrystalMax;
    public int monsterCrystalLeft;

    [Header("Combat state")]
    public CombatState state;
    public Faction faction;

    [Header("Announcement")]
    public GameObject announcementObject;
    public TextMeshProUGUI announcementText;

    [Header("Audio")]
    public AudioManager audioManager;
    public AudioSource audioSource;
    public AudioClip victorySound;
    public AudioClip defeatedSound;
    public AudioClip newRound;

    string nextScene;

    bool playerControl;
    int round;
    float wait;

    private void Start()
    {
        LoadCombatData(GameData.instance.data.combat);
        state = CombatState.ready;
        playerControl = true;

        faction = Faction.Player;
        combatUI.SetHint("Select unit");
        combatUI.SetArmies();

        for (int i = 0; i < armyCrystalMax - armyCrystalsLeft; i++) {
            combatUI.UseCrystal(true);
        }

        for (int i = 0; i < monsterCrystalMax - monsterCrystalLeft; i++)
        {
            combatUI.UseCrystal(false);
        }

        GameData.instance.data.combat = ToString();
        SaveManager.Save(GameData.instance);
    }

    private void Update()
    {
        if (state == CombatState.endTurn)
        {
            if (!unitManager.selectedUnit.animationRunning)
            {
                EndTurn();
            }
        }
        else if (state == CombatState.aiAction)
        {
            if (!unitManager.selectedUnit.moving)
            {
                ai.PerformAction();
            }
        }

        if (playerControl && unitManager.selectedUnit != null && !unitManager.selectedUnit.moving)
        {
            if (Input.GetMouseButtonDown(1))
            {
                unitManager.Unselect();
                if (unitManager.selectedUnit == null)
                {
                    combatUI.HideUnitInspector(true);
                }
            }

            else if (unitManager.selectedUnit != null) {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Action(0, true);
                    Action(0, false);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Action(1, true);
                    Action(1, false);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    Action(2, false);
                    Action(2, true);
                }
                else if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    UndoMovement();
                }
                else if (Input.GetKeyDown(KeyCode.Space)) {
                    EndTurn();
                }
            }
        }
    }

    public void TileHover(Point tile)
    {
        if (playerControl)
        {
            unitManager.TileHover(tile);
            Unit unit = unitManager.GetUnit(tile)?.unit;
            if (unit != null)
            {
                if (unit.faction == Faction.Player)
                {
                    combatUI.InspectUnit(unit, false, false);
                }
                else
                {
                    Inspect(unit, false);
                }
                
            }
        }
    }

    public void TileHoverEnd(Point tile)
    {
        unitManager.TileHoverEnd(tile);
        combatUI.HideEnemyInspector(false);
        combatUI.HideUnitInspector(false);

    }

    public void TileClicked(Point tile)
    {
        // Enemy inspector
        Unit unit = unitManager.GetUnit(tile)?.unit;

        if (unit != null)
        {
            if (unit.faction != Faction.Player) {
                Inspect(unit, true);
            }
            else if (!unit.hasActionLeft)
            {
                Inspect(unit, true);
            }
        }
        else
        {
            HideEnemyInfo();
        }

        // No action is allowed
        if (state == CombatState.waiting){
            return;
        }

        // Player can choose unit
        else if (state == CombatState.ready)
        {
            unitManager.SelectUnit(tile, Faction.Player);
        }

        // Player can move unit or use default skill
        else if (state == CombatState.unit)
        {
            unitManager.WalkTo(tile, Faction.Player);
        }
    }

    /*==================================================================
     *  STATE FUNCTIONS
     * ================================================================*/

    // Player has selected an unit
    public void UnitSelected(Unit unit)
    {
        if (state == CombatState.ready || (state == CombatState.unit))
        {
            combatUI.InspectUnit(unit, state == CombatState.unit, true);
            combatUI.SetHint("Give order");
            state = CombatState.unit;
            playerControl = true;
        }
    }

    // Selected unit is unselected
    public void UnitUnselected()
    {
        if (state == CombatState.unit)
        {
            combatUI.HideUnitInspector(true);
            combatUI.ShowUndoMove(false);
            combatUI.SetHint("Select unit");
            state = CombatState.ready;
            playerControl = true;
        }
    }

    // Unit has moved and can still perform action
    public void MoveDone()
    {
        if (state == CombatState.unit)
        {
            combatUI.InspectUnit(unitManager.selectedUnit.unit, true, true);
            combatUI.SetHint("Perform action");
            Action(0);
        }
        else if (state == CombatState.ai)
        {
            state = CombatState.aiAction;
            playerControl = false;
        }
    }

    // Selected unit has completed it's turn
    public void EndTurn(bool skip = false)
    {
        // Hide inspectors and markings
        combatUI.HideUnitInspector(true);
        combatUI.HideHint();
        mapManager.RemoveMarkings();

        if (state == CombatState.end)
        {
            return;
        }

        // Wait for animations to complete
        if (state != CombatState.endTurn)
        {
            state = CombatState.endTurn;
            playerControl = false;
            return;
        }

        // Consume crystal and end unit's turn
        if (!skip)
        {
            combatUI.UseCrystal(faction == Faction.Player);
            if (faction == Faction.Player)
            {
                armyCrystalsLeft--;
            }
            else
            {
                monsterCrystalLeft--;
            }
            unitManager.EndTurn();
        }

        // Save
        if (faction == Faction.Monster)
        {
            GameData.instance.data.combat = ToString();
            SaveManager.Save(GameData.instance);
        }

        // Next armys turn
        int other = unitManager.ActiveUnits(faction);
        int otherCrystals = faction == Faction.Player ? armyCrystalsLeft : monsterCrystalLeft;
        
        faction = faction == Faction.Player ? Faction.Monster : Faction.Player;

        int current = unitManager.ActiveUnits(faction);
        int currentCrystals = faction == Faction.Player ? armyCrystalsLeft : monsterCrystalLeft;

        // No active units left or out of crystals
        if (current == 0 || currentCrystals == 0)
        {
            // Neither army has any active units left or out of crystals
            if (other == 0 || otherCrystals == 0)
            {
                StartCoroutine(NewRound());
            }

            // Give turn to other army
            else
            {
                EndTurn(true);
            }
        }
        // Perform turn
        else
        {
            // Player turn
            if (faction == Faction.Player)
            {
                combatUI.SetHint("Select unit");
                state = CombatState.ready;
                playerControl = true;
                StartTurn(Faction.Player);
            }

            // Enemy turn
            else
            {
                state = CombatState.ai;
                playerControl = false;
                ai.PerformTurn();
            }
        }
    }

    public void StartTurn(Faction faction)
    {
        if (faction == Faction.Player)
        {
            if (unitManager.ActiveUnits(faction) == 1)
            {
                foreach(UnitPiece piece in unitManager.army)
                {
                    if (piece.unit.hasActionLeft)
                    {
                        unitManager.SelectUnit(piece.unit.position, faction);
                        return;
                    }
                }
            }
        }
    }

    /*==================================================================
    * BUTTONS
    * ================================================================*/

    // Action selected
    public void Action(int index, bool ui = false)
    {
        if (state == CombatState.unit)
        {
            if (ui)
            {
                unitManager.ShowAction(index);
            }
            else
            {
                combatUI.SelectAction(index);
            }
        }
    }

    // Undo movement, only possible before performing an action
    public void UndoMovement()
    {
        if (state == CombatState.unit)
        {
            unitManager.UndoMovement();
            combatUI.InspectUnit(unitManager.selectedUnit.unit, false, true);
            combatUI.ShowUndoMove(false);
        }
    }

    public void CancelAction()
    {
        if (state == CombatState.unit)
        {
            unitManager.ShowMovement();
            unitManager.SelectAction(0);
        }
    }

    public void Inspect(Unit unit, bool clicked)
    {
        combatUI.InspectEnemy(unit, clicked);
    }

    public void HideEnemyInfo()
    {
        combatUI.HideEnemyInspector(true);
    }

    /*==================================================================
    * COMBAT
    * ================================================================*/

    public IEnumerator NewRound(bool firstRound = false)
    {

        // Next round
        round++;
        faction = Faction.Player;
        playerControl = false;
        state = CombatState.waiting;

        // Wait for unitManager
        StartCoroutine(unitManager.NewRound(firstRound));
        yield return new WaitForSeconds(.1f);

        while (unitManager.isActive)
        {
            yield return new WaitForSeconds(.2f);
        }

        announcementObject.SetActive(true);
        announcementText.text = "Round " + round;
        yield return new WaitForSeconds(1);

        announcementObject.SetActive(false);

        state = CombatState.ready;
        playerControl = true;

        // Refresh crystals
        armyCrystalsLeft = armyCrystalMax;
        monsterCrystalLeft = monsterCrystalMax;

        combatUI.NewRound(round);

        GameData.instance.data.combat = ToString();
        SaveManager.Save(GameData.instance);
    }

    public IEnumerator EndCombat(bool victory)
    {
        state = CombatState.end;
        playerControl = false;

        audioManager.StopMusic();
        audioSource.PlayOneShot(victory ? victorySound : defeatedSound);

        nextScene = victory ? "MapScene" : "MenuScene";

        yield return new WaitForSeconds(1f);
        combatUI.ShowResult(victory ? "VICTORY!" : "DEFEATED!");
        yield return new WaitForSeconds(victory ? 3f : 5f);
        combatUI.HideResult();

        if (victory && unitManager.fallenUnits.Count > 0)
        {
            graveyard.Create();
        }
        else
        {
            ExitCombat();
        }
    }

    public void ExitCombat()
    {
        GameData.instance.data.RemoveFallen(unitManager.fallenUnits);
        GameData.instance.data.combat = "";
        // ToDo: Remove dead units from army
        GameData.instance.LoadScene(nextScene);
    }

    /*==================================================================
    * LOAD
    * ================================================================*/

    private void LoadCombatData(string data)
    {
        if (data.Length > 10)
        {
            string[] lines = data.Split('\n');
            string[] parts = lines[0].Split();
            int i = 0;
            round = int.Parse(parts[i++]);
            armyCrystalMax = int.Parse(parts[i++]);
            armyCrystalsLeft = int.Parse(parts[i++]);
            monsterCrystalMax = int.Parse(parts[i++]);
            monsterCrystalLeft = int.Parse(parts[i++]);

            mapManager.Load(lines[1]);

            unitManager.Load(mapManager, lines[2], lines[3]);

            if (unitManager.monsters.Count == 0)
            {
                state = CombatState.end;
                playerControl = false;
                nextScene = "MapScene";
                graveyard.Create();
            }
            else
            {
                combatUI.SetRound(round);
            }

        }
        else
        {
            round = 0;
            armyCrystalMax = armyCrystalsLeft = GameData.instance.data.maxCrystals;

            Combat combat = GameData.instance.combat.CreateInstance(GameData.instance.combatDifficulty);

            monsterCrystalMax = monsterCrystalLeft = combat.monsterCrystals;

            mapManager.Load(combat);
            unitManager.Load(mapManager, GameData.instance.data.army.ToArray(), combat.monsters);
            StartCoroutine(NewRound(true));
        }
    }

    public override string ToString()
    {
        string str = "";
        str += round + " " + armyCrystalMax + " " + armyCrystalsLeft + " " + monsterCrystalMax + " " + monsterCrystalLeft + "\n";
        str += mapManager.ToString();
        str += unitManager.ToString();
        return str;
    }
}

public enum CombatState {
    waiting,    // Player cant give commands
    ready,      // Player can choose unit
    unit,       // Unit is selected and can move or perform action
    endTurn,    // Ending turn once animatin has finished
    end,        // Combat has ended
    ai,         // AI is taking turn
    aiAction    // AI can perform action
}