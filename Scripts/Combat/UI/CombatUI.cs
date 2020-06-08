using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUI : MonoBehaviour
{
    public CombatManager combatManager;
    public TextMeshProUGUI roundNumber;

    [Header("Armies")]
    public ArmyRibbon armyRibbon;
    public CrystalManager armyCrystals;
    public CrystalManager monsterCrystals;

    [Header("Unit")]
    public UnitInspector unitInspector;
    public UnitInspector enemyInspector;
    public ActionUIManager actionUI;

    [Header("Hint")]
    public GameObject hintObject;
    public TextMeshProUGUI hintText;

    [Header("Icons")]
    public Sprite[] icons;

    [Header("CombatOver")]
    public GameObject overlayObject;
    public TextMeshProUGUI resultText;

    [Header("Hover info")]
    public GameObject hoverBox;
    public TextMeshProUGUI hoverText;
    public Image hoverEnergy;

    private void Start()
    {
        HideUnitInspector(true);
        roundNumber.text = "1";
        unitInspector.ui = this;
        enemyInspector.ui = this;
    }

    public void SetArmies()
    {
        armyRibbon.SetArmy(combatManager.unitManager.army);

        armyCrystals.SetCrystals(combatManager.armyCrystalMax);
        monsterCrystals.SetCrystals(combatManager.monsterCrystalMax);
    }

    /*================================================
     INSPECTORS
    ================================================*/

    public void InspectUnit(Unit unit, bool hasMoved, bool strong)
    {
        if (unit != null)
        {
            unitInspector.SetUnit(unit, strong);
            if (strong)
            {
                actionUI.SetUnit(unit, hasMoved);
            }
        }
    }

    public void InspectEnemy(Unit unit, bool strong)
    {
        enemyInspector.SetUnit(unit, strong);
    }

    public void HideUnitInspector(bool strong)
    {
        unitInspector.Hide(strong);
        enemyInspector.Hide(strong);
        if (strong)
        {
            actionUI.Hide();
        }
    }

    public void HideEnemyInspector(bool strong)
    {
        enemyInspector.Hide(strong);
    }

    public void ShowUndoMove(bool show)
    {
        actionUI.ShowUndoMove(show);
    }

    /*================================================
     BUTTON INPUT
    ================================================*/

    public void SelectAction(int index)
    {
        actionUI.Action(index);
    }

    public void ActionSelected(int index)
    {
        combatManager.Action(index, true);
    }

    public void ActionUnselected()
    {
        combatManager.unitManager.ShowMovement();
    }

    public void UndoMovement()
    {
        combatManager.UndoMovement();
    }

    public void EndTurn()
    {
        combatManager.EndTurn();
    }

    /*================================================
     ACTION HOVER
    ================================================*/

    public void ActionHover(Action action)
    {
        hoverBox.SetActive(true);
        hoverBox.transform.position = Input.mousePosition + new Vector3(10,0);
        hoverText.text = "<u>" + action.actionName + "</u>\n";
        hoverText.text += action.GetDescription();

        SetHoverBoxSize();
    }

    public void PassiveHover(Passive passive)
    {
        hoverBox.SetActive(true);
        hoverBox.transform.position = Input.mousePosition + new Vector3(10, 0);
        hoverText.text = "<u>" + passive.passiveName + "</u>\n";
        hoverText.text += passive.GetDescription();

        SetHoverBoxSize();
    }

    private void SetHoverBoxSize()
    {
        RectTransform rt = hoverBox.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(Mathf.Max(hoverText.preferredWidth + 10, 100), Mathf.Max(hoverText.preferredHeight + 10, 50));

        if (rt.sizeDelta.x + hoverBox.transform.position.x > 800)
        {
            hoverBox.transform.position = Input.mousePosition - new Vector3(rt.sizeDelta.x + 25, 0);
        }

        hoverEnergy.gameObject.SetActive(false);
    }

    public void ActionHide()
    {
        hoverBox.SetActive(false);
        combatManager.unitManager.ShowCurrent();
    }

    /*================================================
     CRYSTALS
    ================================================*/

    public void SetRound(int round)
    {
        roundNumber.text = round.ToString();
    }

    public void NewRound(int round)
    {
        armyCrystals.RefreshCrystals();
        monsterCrystals.RefreshCrystals();
        roundNumber.text = round.ToString();
        SetHint("NEW ROUND!");
    }

    public void UseCrystal(bool player)
    {
        if (player)
        {
            armyCrystals.UseCrystal();
        }
        else
        {
            monsterCrystals.UseCrystal();
        }
    }

    /*================================================
     HINTS
    ================================================*/

    public void SetHint(string text)
    {
        hintText.text = text;
        hintObject.SetActive(true);
    }

    public void HideHint()
    {
        hintObject.SetActive(false);
    }

    public void ShowResult(string message)
    {
        resultText.text = message;
        overlayObject.SetActive(true);
    }

    public void HideResult()
    {
        overlayObject.SetActive(false);
    }
}