using UnityEngine;
using TMPro;

public class MapUI : MonoBehaviour
{
    [Header("Resources")]
    public TextMeshProUGUI resources;
    public TextMeshProUGUI coins;
    public TextMeshProUGUI days;

    public Color[] resourceColor;

    [Header("DefeatedScreen")]
    public GameObject gameOverContainer;
    public TextMeshProUGUI defeatedReason;

    [Header("Info")]
    public TextMeshProUGUI info;

    private void Start()
    {
        info.text = "Seed: " + GameData.instance.data.seed;
    }

    public void UpdateAll()
    {
        UpdateDate();
        UpdateResources();
        UpdateCoin();
    }

    public void UpdateResources()
    {
        int res = GameData.instance.data.resources;
        int max = GameData.instance.data.maxResources;
        resources.text = res + " / " + max;
        if (res == 0)
        {
            resources.color = resourceColor[2];
        }
        else if (res == max)
        {
            resources.color = resourceColor[0];
        }
        else
        {
            resources.color = resourceColor[1];
        }
        
    }

    public void UpdateDate()
    {
        days.text = "Day " + GameData.instance.data.day;
    }
    public void UpdateCoin()
    {
        coins.text = GameData.instance.data.coins.ToString();
    }

    public void ShowGameOver(string message)
    {
        gameOverContainer.SetActive(true);
        defeatedReason.text = message;
    }
}
