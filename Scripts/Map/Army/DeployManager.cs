using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeployManager : MonoBehaviour
{
    public Image hoverImage;
    public int width;
    public DeployTile[] tiles;

    Unit selectedUnit;
    DeployTile lastTile;

    public void SetUnits()
    {
        for(int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Init(this, new Point(i % width, i / width));
        }

        Debug.Log("ArmySize: " + GameData.instance.data.army.Count);
        foreach (Unit unit in GameData.instance.data.army)
        {
            int i = unit.position.x + unit.position.y * width;
            Debug.Log(unit.unitName + " to index " + i + " " + unit.position);
            tiles[i].SetUnit(unit);
        }
    }
    private void Update()
    {
        hoverImage.transform.position = Input.mousePosition;
        if (Input.GetMouseButtonUp(0) && selectedUnit != null)
        {
            foreach(DeployTile dt in tiles)
            {
                if (dt.hovering)
                {
                    return;
                }
            }
            MouseUp(lastTile);
            lastTile = null;
        }
    }

    public void MouseDown(DeployTile tile)
    {
        lastTile = tile;

        Unit u = tile.unit;
        tile.SetUnit(selectedUnit);
        selectedUnit = u;

        hoverImage.sprite = selectedUnit?.sprite;
        hoverImage.gameObject.SetActive(selectedUnit != null);
    }

    public void MouseUp(DeployTile tile)
    {
        lastTile = tile;

        Unit u = selectedUnit;
        selectedUnit = tile.unit;
        tile.SetUnit(u);

        hoverImage.sprite = selectedUnit?.sprite;
        hoverImage.gameObject.SetActive(selectedUnit != null);

    }

}
