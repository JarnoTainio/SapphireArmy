using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionInfo : MonoBehaviour
{
    public Image icon;
    public Action action;
    UnitInspector inspector;

    public void SetAction(UnitInspector inspector, Action action)
    {
        this.inspector = inspector;
        if (action != null)
        {
            this.action = action;
            gameObject.SetActive(true);
            icon.sprite = action.icon;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
