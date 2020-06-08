using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    public static GameData instance;
    public string[] scenes;

    [Header("Player info")]
    public int saveIndex;

    [Header("Resources")]
    public Data data;

    [Header("Combat")]
    public int combatDifficulty;
    public Combat combat;

    [Header("Audio")]
    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;

    public bool paused;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InstantiateArmy(data.army);
        }
    }
    public void NewProfile()
    {
        data.level = 1;
        data.experience = 0;
    }

    public void NewGame()
    {
        data.NewGame();
    }

    private void InstantiateArmy(List<Unit> army)
    {
        for (int i = 0; i < army.Count; i++)
        {
            army[i] = Instantiate(army[i]);
        }
    }

    public void LoadScene(string scene)
    {
        
        paused = false;

        StartCoroutine(Loading(scene));
    }

    private IEnumerator Loading(string scene)
    {
        GameObject go = GameObject.Find("Canvas");
        Debug.Log(go);
        FadeoutPanel panel = go.GetComponentInChildren<FadeoutPanel>(true);

        if (panel != null)
        {
            StartCoroutine(panel.FadeOut(scene));
            while (panel.fading)
            {
                yield return null;
            }
        }

        if (scene != "MenuScene")
        {
            data.scene = scene;
        }
        SceneManager.LoadScene(scene);
    }

    // AUDIO

    public void SetMasterVolume(float f)
    {
        masterVolume = f;
    }

    public void SetMusicColume(float f)
    {
        musicVolume = f;
    }

    public void SetEfectVolume(float f)
    {
        sfxVolume = f;
    }

    public float GetEffectVolume()
    {
        return sfxVolume * masterVolume;
    }
}
