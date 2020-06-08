using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitInspector : MonoBehaviour
{
    public GameObject unitContainer;
    public TextMeshProUGUI unitName;

    [Header("Images")]
    public Image lifeImage;
    public Image energyImage;
    public Image armorImage;

    [Header("TextFields")]
    public TextMeshProUGUI life;
    public TextMeshProUGUI energy;
    public TextMeshProUGUI armor;
    public TextMeshProUGUI move;

    [Header("Actions")]
    public ActionInfo[] actions;

    [Header("Passives")]
    public PassiveInfo[] passives;

    [HideInInspector]
    public CombatUI ui;

    bool strongInspection;

    public void SetUnit(Unit unit, bool strong)
    {
        if (!strongInspection || strong)
        {
            strongInspection = strongInspection || strong;
            unitContainer.SetActive(unit != null);
            unitName.text = unit.unitName;

            life.text = unit.life + " / " + unit.GetMaxLife();

            energy.text = unit.energy + " / " + unit.GetMaxEnergy();
            energyImage.gameObject.SetActive(unit.maxEnergy > 0);

            armor.text = unit.armor + " / " + unit.GetMaxArmor();
            armorImage.gameObject.SetActive(unit.maxArmor > 0);

            move.text = unit.move + "";

            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].SetAction(this, unit.actions[i]);
            }

            for (int i = 0; i < passives.Length; i++)
            {
                passives[i].SetPassive(this, unit.passives[i]);
            }
        }
    }

    public void Hide(bool strong)
    {
        if (strong || !strongInspection)
        {
            strongInspection = false;
            strongInspection = false;
            unitContainer.SetActive(false);
        }
    }

    public void ActionHover(ActionInfo action)
    {
        ui.ActionHover(action.action);
    }

    public void PassiveHover(PassiveInfo passive)
    {
        ui.PassiveHover(passive.passive);
    }

    public void HoverEnd()
    {
        ui.ActionHide();
    }
}
