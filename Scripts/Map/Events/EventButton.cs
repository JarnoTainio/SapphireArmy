using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EventButton : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI text;

    public GameObject hintObject;
    public TextMeshProUGUI hintText;

    [HideInInspector]
    public Event islandEvent;

    bool hovering;

    public void SetEvent(Event islandEvent)
    {
        this.islandEvent = islandEvent;
        image.sprite = islandEvent.icon;
        image.gameObject.SetActive(image.sprite != null);

        text.text = islandEvent.buttonText;
        
        foreach(EventEffect effect in islandEvent.effects)
        {
            if (!effect.CanBeDone())
            {
                GetComponent<Button>().interactable = false;
                image.color = Color.gray;
                return;
            }
        }

        GetComponent<Button>().interactable = true;
        image.color = Color.white;
    }

    private void Update()
    {
        if (hovering)
        {
            hintObject.transform.position = Input.mousePosition + new Vector3(25, 0 ,0);
        }
    }

    public void MouseEnter()
    {
        string hint = islandEvent.GetHint();
        if (hint.Length > 0)
        {
            hintObject.SetActive(true);
            hintText.text = hint;
            hovering = true;
        }
    }

    public void MouseExit()
    {
        hintObject.SetActive(false);
        hovering = false;
    }
}
