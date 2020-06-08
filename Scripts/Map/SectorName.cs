using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SectorName : MonoBehaviour
{
    public TextMeshProUGUI sectorName;
    public Image[] difficultyImages;
    public Sprite[] difficulties;

    float duration;
    private void Update()
    {
        if (duration > 0)
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void Show(Sector sector)
    {
        sectorName.text = sector.sectorName;
        int difficulty = sector.GetDifficulty();
        for(int i = 0; i < difficultyImages.Length; i++)
        {
            if (i < difficulty)
            {
                difficultyImages[i].gameObject.SetActive(true);
                int index = (difficulty - i) / difficultyImages.Length;
                index = Mathf.Min(index, difficulties.Length - 1);
                Debug.Log(i + ":" + index);
                difficultyImages[i].sprite = difficulties[index];
            }
            else
            {
                difficultyImages[i].gameObject.SetActive(false);
            }
        }

        gameObject.SetActive(true);
        duration = 2.5f;
    }


}
