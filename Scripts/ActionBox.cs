using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBox : MonoBehaviour
{
    public Image image;
    public string description;

    public void Set(Sprite icon, string description)
    {
        image.sprite = icon;
        this.description = description;
    }
}
