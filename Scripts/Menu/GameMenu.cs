using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{

    public GameObject menuContainer;

    public void Quit()
    {
        SaveManager.Save(GameData.instance);
        GameData.instance.LoadScene("MenuScene");
    }

    public void ToggleMenu()
    {
        menuContainer.SetActive(!menuContainer.activeSelf);
        GameData.instance.paused = menuContainer.activeSelf;
    }

    public void ShowMenu(bool open = true)
    {
        GameData.instance.paused = true;
        menuContainer.SetActive(open);
    }
}
