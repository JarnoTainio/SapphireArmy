using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CombatCamera : MonoBehaviour
{
    public CombatMap map;
    public float speed = 1f;

    bool dragging;
    Vector3 dragPosition;

    private void Start()
    {
        transform.localPosition = new Vector3(map.width / 2, map.height / 2, transform.localPosition.z);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Move(new Vector3(0, 1));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Move(new Vector3(0, -1));
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            Move(new Vector3(-1, 0));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Move(new Vector3(1, 0));
        }

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            dragging = true;
            dragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }

        if (dragging)
        {
            Vector3 newPos = transform.localPosition + (dragPosition - Camera.main.ScreenToWorldPoint(Input.mousePosition)) / 1f;
            newPos.z = transform.localPosition.z;
            transform.localPosition = newPos;
            dragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CheckBounds();
        }

    }

    private void CheckBounds()
    {
        Vector3 pos = transform.localPosition;
        if (pos.x < 0)
        {
            pos.x = 0;
        }
        else if (pos.x >= map.width)
        {
            pos.x = map.width;
        }

        if (pos.y < 0)
        {
            pos.y = 0;
        }
        else if (pos.y >= map.height)
        {
            pos.y = map.height;
        }
        transform.localPosition = pos;
    }
    public void Move(Vector3 direction)
    {
        transform.localPosition += direction * speed * Time.deltaTime;
        CheckBounds();
    }
}
