using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionUIManager : MonoBehaviour
{
    public CombatUI combatUI;

    [Header("Description")]
    public GameObject descriptionBox;
    public TextMeshProUGUI description;
    
    [Header("Actions")]
    public GameObject actionContainer;
    public ActionButton[] actionButtons;
    public Button undoButton;

    int currentAction;

    private void Start()
    {
        descriptionBox.SetActive(false);
        actionContainer.SetActive(false);
    }

    public void SetUnit(Unit unit, bool hasMoved)
    {
        actionContainer.gameObject.SetActive(true);
        descriptionBox.SetActive(false);

        actionContainer.SetActive(true);
        ShowUndoMove(hasMoved);

        for (int i = 0; i < actionButtons.Length; i++)
        {
            if (i < unit.actions.Length && unit.actions[i] != null)
            {
                actionButtons[i].gameObject.SetActive(true);
                actionButtons[i].SetAction(unit.actions[i]);
                actionButtons[i].Selected(false);
            }
            else
            {
                actionButtons[i].gameObject.SetActive(false);
            }
        }
        currentAction = -1;
    }

    public void Action(int index)
    {
        if (actionButtons[index].action == null)
        {
            return;
        }

        if (index == currentAction)
        {
            descriptionBox.SetActive(false);
            actionButtons[index].Selected(false);
            combatUI.ActionUnselected();
            currentAction = -1;
            return;
        }

        currentAction = index;

        for (int i = 0; i < actionButtons.Length; i++)
        {
            actionButtons[i].Selected(index == i);
        }
        descriptionBox.SetActive(true);
        Action action = actionButtons[index].action;
        description.text = "<u>" + action.actionName + "</u>\n" + action.GetDescription();
        combatUI.ActionSelected(index);
    }

    public void ShowUndoMove(bool show = true)
    {
        undoButton.gameObject.SetActive(show);
    }

    public void Hide()
    {
        actionContainer.gameObject.SetActive(false);
        descriptionBox.SetActive(false);
    }

    public void SkipTurn() 
    {
        combatUI.EndTurn();
    }
}
