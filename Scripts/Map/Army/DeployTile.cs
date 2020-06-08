using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeployTile : MonoBehaviour
{
    public Unit unit;
    public Image tileImage;
    public Image unitImage;

    DeployManager manager;
    Point point;

    public bool hovering;

    public void Init(DeployManager manager, Point point)
    {
        this.manager = manager;
        this.point = point;
    }

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
        if (unit == null)
        {
            unitImage.gameObject.SetActive(false);
        }
        else
        {
            unit.position = new Point(point.x , point.y);
            unitImage.sprite = unit.sprite;
            unitImage.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (hovering)
        {
            if (Input.GetMouseButtonUp(0))
            {
                manager.MouseUp(this);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                manager.MouseDown(this);
            }
        }
    }

    public void OnMouseEnter()
    {
        hovering = true;
    }

    public void OnMouseExit()
    {
        hovering = false;
    }
}
