using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionButton : MonoBehaviour
{
    public Image background;
    public Image icon;
    public Image energyIcon;

    public Action action;

    public void SetAction(Action action)
    {
        this.action = action;
        icon.sprite = action.icon;
        energyIcon.gameObject.SetActive(action.cost > 0);
        SetUsable(action.CanUse());
    }

    public void Selected(bool selected)
    {
        if (action != null)
        {
            background.color = 
                selected ? 
                (action.CanUse() ? Color.yellow : Color.red) 
                    : 
                (action.CanUse() ? Color.white : Color.grey);
        }
    }

    public void SetUsable(bool isUsable)
    {
        background.color = isUsable ? Color.white : Color.gray;
        icon.color = isUsable ? Color.white : Color.gray;
        energyIcon.color = isUsable ? Color.white : Color.grey;
    }
}
