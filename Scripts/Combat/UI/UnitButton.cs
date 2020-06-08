using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour
{
    public Image background;
    public Image unitImage;

    public void SetUnit(UnitPiece piece)
    {
        gameObject.SetActive(true);
        //unitImage = image;
    }
}
