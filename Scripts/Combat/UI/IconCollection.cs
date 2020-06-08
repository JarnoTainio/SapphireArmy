using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconCollection : MonoBehaviour
{
    public Image[] images;
    
    public void Set(int value, int max)
    {
        for(int i = 0; i < images.Length; i++)
        {
            if (i < max) {
                images[i].gameObject.SetActive(true);
                images[i].color = (i < value ? Color.white : Color.black);
            }
            else
            {
                images[i].gameObject.SetActive(false);
            }
        }
    }
}
