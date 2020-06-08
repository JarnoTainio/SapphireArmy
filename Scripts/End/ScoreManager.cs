using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    [Header("Score")]
    public TextMeshProUGUI scoreNames;
    public TextMeshProUGUI scoreCount;
    public TextMeshProUGUI scorePoints;
    public TextMeshProUGUI scoreTotal;

    [Header("Experience")]
    public TextMeshProUGUI experienceText;
    public TextMeshProUGUI levelText;
    public Image oldExperienceBar;
    public Image newExperienceBar;

    [Header("Value")]
    public int islandValue = 10;
    public int combatValue = 10;
    public int goldValue = 1;

    [Header("Reward")]
    public TextMeshProUGUI rewardText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip levelUpSound;
    public AudioSource experienceSource;
    public AudioClip smallXpSound;
    public AudioClip mediumXpSound;

    [Header("Settings")]
    public float timePerCategory = 2f;
    int oldExperience;
    int newExperience;
    int experience;
    int required;

    public void Start()
    {
        levelText.text = GameData.instance.data.level.ToString();
        experienceSource.volume = GameData.instance.GetEffectVolume() * .75f;
        StartCoroutine(Score());
    }

    private IEnumerator Score()
    {
        rewardText.text = "";
        Data data = GameData.instance.data;
        
        oldExperience = data.experience;
        newExperience = 0;
        experience = 0;
        required = GetLevelRequirement();

        UpdateExperienceBar(oldExperience, newExperience, required);

        string names = "";
        string count = "";
        string points = "";
        int total = 0;

        // Visited islands
        names += "Visited islands (" + islandValue + ")";
        scoreNames.text = names;
        int num = 0;


        float speed = timePerCategory / data.visitedIslands.Count;
        experienceSource.clip = mediumXpSound;
        foreach(Point point in data.visitedIslands)
        {
            experienceSource.Play();
            num++;
            experience += islandValue;

            count = "x" + num;
            scoreCount.text = count;

            points = num * islandValue + "";
            scorePoints.text = points;

            total = num * islandValue;
            scoreTotal.text = total.ToString();

            // Level up
            if (experience >= required)
            {
                LevelUp();
            }

            UpdateExperienceBar(oldExperience, experience, required);
            yield return new WaitForSeconds(speed);
        }

        // Combats won

        // Gold
        names += "\nGold (" + goldValue + ")";
        scoreNames.text = names;
        int gold = 0;
        speed = timePerCategory / GameData.instance.data.coins;
        experienceSource.clip = smallXpSound;
        while (gold < GameData.instance.data.coins)
        {
            experienceSource.Play();
            gold++;

            experience += goldValue;
            scoreCount.text = count + "\nx" + gold;

            scorePoints.text = points + "\n" + (gold * goldValue);

            total += goldValue;
            scoreTotal.text = total.ToString();

            // Level up
            if (experience >= required)
            {
                LevelUp();
            }

            UpdateExperienceBar(oldExperience, experience, required);

            yield return new WaitForSeconds(speed);
        }
        count += "\nx" + gold;
        points += "\n" + (gold * goldValue);

        data.coins = 0;

        GameData.instance.NewGame();
        SaveManager.Save(GameData.instance);
    }

    private void LevelUp()
    {
        audioSource.volume = GameData.instance.GetEffectVolume();
        audioSource.PlayOneShot(levelUpSound);

        oldExperience = 0;
        experience -= required;

        // Update data
        GameData.instance.data.level++;
        GameData.instance.data.experience = experience;

        // Update required
        required = GetLevelRequirement();

        // Level
        levelText.text = GameData.instance.data.level.ToString();

        // Reward text
        if (rewardText.text.Length > 0)
        {
            rewardText.text += "\n";
        }
        rewardText.text += GameData.instance.data.LevelReward(GameData.instance.data.level);
    }

    private void UpdateExperienceBar(float experience, float bonus, float required)
    {
        float current = experience + bonus;
        oldExperienceBar.transform.localScale = new Vector3(experience / required, 1f);
        newExperienceBar.transform.localScale = new Vector3(current / required, 1f);
        experienceText.text = current + " / " + required;
    }

    public void Quit()
    {
        GameData.instance.LoadScene("MenuScene");
    }

    public void NewRun()
    {
        GameData.instance.NewGame();
        GameData.instance.LoadScene("MapScene");
    }

    private int GetLevelRequirement()
    {
        return (GameData.instance.data.level) * (GameData.instance.data.level) * 50 + 50;
    }
}
