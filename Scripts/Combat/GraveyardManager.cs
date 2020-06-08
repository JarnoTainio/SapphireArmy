using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraveyardManager : MonoBehaviour
{
    public CombatManager combatManager;
    public UnitManager unitManager;

    public UnitBox unitBoxPrefab;
    public GameObject unitContainer;
    public TextMeshProUGUI resourcesText;

    List<UnitBox> unitBoxes;
    List<int> revivedUnits;

    int resources;

    private void Start()
    {
        
    }

    public void Create()
    {
        gameObject.SetActive(true);
        List<Unit> army = GameData.instance.data.army;
        unitBoxes = new List<UnitBox>();
        for (int i = 0; i < army.Count; i++)
        {
            UnitBox box = Instantiate(unitBoxPrefab, unitContainer.transform);
            box.SetUnit(army[i], i, !unitManager.fallenUnits.Contains(i));
            box.graveyard = this;
            unitBoxes.Add(box);
        }
        Undo();
    }

    public void Undo()
    {
        resources = GameData.instance.data.resources;
        resourcesText.text = resources + " / " + GameData.instance.data.maxResources;
        revivedUnits = new List<int>();

        List<Unit> army = GameData.instance.data.army;
        for (int i = 0; i < army.Count; i++)
        {
            unitBoxes[i].SetUnit(army[i], i, !unitManager.fallenUnits.Contains(i));
            unitBoxes[i].CanBeRevived(resources >= unitBoxes[i].reviveCost);
        }
    }

    public void Revive(int index)
    {
        revivedUnits.Add(index);
        resources -= unitBoxes[index].reviveCost;
        resourcesText.text = resources + " / " + GameData.instance.data.maxResources;
        foreach(UnitBox box in unitBoxes)
        {
            box.CanBeRevived(resources >= box.reviveCost);
        }
    }

    public void Confirm()
    {
        foreach(int i in revivedUnits)
        {
            unitManager.fallenUnits.Remove(i);
        }
        GameData.instance.data.resources = resources;
        combatManager.ExitCombat();
    }
}
