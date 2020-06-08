using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatToPercentage : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void SetValue(float f)
    {
        text.text = (int)(f * 100) + "%";
    }
}
