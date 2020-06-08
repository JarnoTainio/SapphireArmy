using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI textObject;
    public Vector3 vector;
    public float lifeTime;

    private void Update()
    {
        transform.localPosition += vector * Time.deltaTime;
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0f)
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string text)
    {
        textObject.text = text;
    }

    public void SetColor(Color color)
    {
        textObject.color = color;
    }

    public void SetFont(int fontSize)
    {
        textObject.fontSize = fontSize;
    }

    public void SetVelocity(Vector2 vec)
    {
        vector = vec;
    }

    public void SetLifeTime(float time)
    {
        lifeTime = time;
    }

    public void SetPosition(Vector3 position)
    {
        transform.localPosition = position;
    }
}
