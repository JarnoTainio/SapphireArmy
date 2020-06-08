using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Data.Common;

public class PlayButton : MonoBehaviour
{
    public TextMeshProUGUI gameText;
    public GameObject deleteButton;

    public void SetData(int index, bool hasData)
    {
        deleteButton.SetActive(hasData);
        if (hasData)
        {
            Data data = SaveManager.Load(index);
            string str = "Level " + data.level + " Islands " + data.visitedIslands.Count;
            str += "\nResources " + data.resources + "/" + data.maxResources + " Gold " + data.coins;
            gameText.text = str;
        }
        else
        {
            gameText.text = "NEW GAME";
        }
    }
}
