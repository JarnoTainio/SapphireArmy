using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public Transform target;
    public float followSpeed;
    public Vector3 offSet;

    private void Update()
    {
        transform.position = offSet + target.position * followSpeed;
    }
}
