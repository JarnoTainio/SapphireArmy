using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEffect : MonoBehaviour
{
    public float speed = 1f;
    Vector3 targetSize;

    private void Update()
    {
        if (targetSize != Vector3.zero)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, Time.deltaTime * speed);
            if (transform.localScale == targetSize)
            {
                targetSize = Vector3.zero;
            }
        }
        else if (transform.localScale.x != 1f)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, Time.deltaTime * speed);
        }
    }

    public void TargetSize(float size, float speed = 1f)
    {
        this.speed = speed;
        targetSize = new Vector3(size, size, 1f);
    }
}
