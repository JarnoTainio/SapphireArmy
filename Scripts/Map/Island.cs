using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Island : MonoBehaviour
{
    public MapManager mapManager;
    public SpriteRenderer islandSprite;

    [Header("Icons")]
    public SpriteRenderer icon;
    public Sprite combatIcon;
    public Sprite unknownIcon;
    public Sprite eventIcon;

    [HideInInspector]
    public Point point;
    [HideInInspector]
    public Event islandEvent;

    public void SetEvent(Event newEvent, bool isUnknown = false)
    {
        islandEvent = newEvent;
        icon.gameObject.SetActive(islandEvent != null);
        if (islandEvent != null)
        {
            if (isUnknown)
            {
                icon.sprite = unknownIcon;
            }
            else{
                switch (newEvent.type)
                {
                    case EventType.Combat:
                        {
                            icon.sprite = combatIcon;
                            break;
                        }
                    case EventType.Event:
                        {
                            icon.sprite = eventIcon;
                            break;
                        }
                }
            }
        }
    }

    public void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            mapManager.IslandClicked(this);
        }
    }

    public void Visited()
    {
        icon.gameObject.SetActive(false);
    }

    public void RemoveEvent()
    {
        islandEvent = null;
        icon.gameObject.SetActive(false);
    }

    public void IsAdjacent(bool isAdjacent)
    {
        GetComponent<SpriteRenderer>().color = isAdjacent ? Color.white : Color.gray;
    }

    public void SetSprite(Sprite sprite)
    {
        islandSprite.sprite = sprite;
    }

}
