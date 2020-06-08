using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public Sprite horizontalStoneBridge;
    public Sprite verticalStoneBridge;

    public Sprite horizontalWeakBridge;
    public Sprite verticalWeakBridge;

    public Point point;
    public bool fragile;
    public void SetSprite(bool strongBridge, bool horizontal)
    {
        fragile = !strongBridge;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (strongBridge)
        {
            sr.sprite = horizontal ? horizontalStoneBridge : verticalStoneBridge;
        }
        else
        {
            sr.sprite = horizontal ? horizontalWeakBridge : verticalWeakBridge;
        }

        point = new Point(transform.localPosition);
    }

    public void Walk()
    {
        if (fragile)
        {
            Destroy(gameObject);
        }
    }
}
