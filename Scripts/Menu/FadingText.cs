using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class FadingText : MonoBehaviour
{

    public TextMeshProUGUI text;
    public float fadeSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        if (text.alpha > 0f)
        {
            text.alpha -= Time.deltaTime * fadeSpeed;
        }
    }

    public void SetText(string str, Color color)
    {
        text.text = str;
        text.alpha = 1f;
        text.color = color;
    }
}
