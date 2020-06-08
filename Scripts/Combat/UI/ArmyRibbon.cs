using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyRibbon : MonoBehaviour
{
    public CombatUI CombatUI;
    public UnitButton[] unitButtons;

    public void SetArmy(List<UnitPiece> army)
    {
        for(int i = 0; i < unitButtons.Length; i++)
        {
            unitButtons[i].gameObject.SetActive(i < army.Count);
            if (i < army.Count)
            {
                unitButtons[i].SetUnit(army[i]);
            }
        }
    }

    public void UnitButton(int index)
    {
        // ToDo: Unit button clicked, focus camera to the unit
    }
}
