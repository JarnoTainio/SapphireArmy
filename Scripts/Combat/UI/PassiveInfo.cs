using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveInfo : MonoBehaviour
{
    public Image icon;
    public Passive passive;
    UnitInspector inspector;

    public void SetPassive(UnitInspector inspector, Passive passive)
    {
        this.inspector = inspector;
        if (passive != null)
        {
            this.passive = passive;
            gameObject.SetActive(true);
            icon.sprite = passive.icon;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
