using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MenuManager : MonoBehaviour
{
    public string version; 
    [Header("PlayButtons")]
    public PlayButton[] playButtons;

    [Header("MenuContainers")]
    public GameObject currentMenu;
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject playMenu;

    [Header("Message")]
    public FadingText message;

    private void Start()
    {
        mainMenu.SetActive(true);
        MainMenu();
        for (int i = 0; i < playButtons.Length; i++)
        {
            playButtons[i].SetData(i, SaveManager.SaveExists(i));
        }
        message.SetText("Version: " + version, Color.green);
    }

    public void MainMenu() 
    {
        playMenu.SetActive(false);
        optionsMenu.SetActive(false);
        currentMenu = null;
    }

    public void PlayMenu()
    {
        if (currentMenu == playMenu)
        {
            MainMenu();
        }
        else
        {
            currentMenu = playMenu;
            playMenu.SetActive(true);
            optionsMenu.SetActive(false);
        }
    }

    public void OptionsMenu()
    {
        if (currentMenu == optionsMenu)
        {
            MainMenu();
        }
        else
        {
            currentMenu = optionsMenu;
            playMenu.SetActive(false);
            optionsMenu.SetActive(true);
        }
    }

    public void LoadGame(int index)
    {
        GameData.instance.saveIndex = index;
        if (SaveManager.SaveExists(index))
        {
            bool gameLoaded = SaveManager.Load(index, GameData.instance);
            if (gameLoaded)
            {
                try
                {
                    if (GameData.instance.data.scene == "EndScene")
                    {
                        GameData.instance.NewGame();
                    }

                    GameData.instance.LoadScene(GameData.instance.data.scene);
                    message.SetText("Loading..", Color.green);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    message.SetText("Failed to load the game...", Color.red);
                }
            }
            else
            {
                message.SetText("Failed to load the game...", Color.red);
            }
        }
        else
        {
            try
            {
                GameData.instance.NewProfile();
                GameData.instance.NewGame();
                GameData.instance.LoadScene("MapScene");
                message.SetText("Loading..", Color.green);
            }
            catch(Exception e)
            {
                Debug.Log(e);
                message.SetText("Failed to load the game...", Color.red);
            }
        }
    }

    public void DeleteSave(int index)
    {
        bool gameDeleted = SaveManager.Delete(index);
        if (gameDeleted)
        {
            playButtons[index].SetData(index, false);
            message.SetText("Save deleted", Color.white);
        }
        else
        {
            message.SetText("Failed to delete the save..", Color.red);
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
