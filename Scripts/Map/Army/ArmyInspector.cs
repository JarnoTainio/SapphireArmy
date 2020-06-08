using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyInspector : MonoBehaviour
{

    public DeployManager deployManager;
    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        if (gameObject.activeSelf)
        {
            deployManager.SetUnits();
        }
    }
}
