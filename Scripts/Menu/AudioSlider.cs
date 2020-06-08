using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI text;
    public AudioType audioType;

    private void Start()
    {
        if (audioType == AudioType.master)
        {
            text.text = "Master Volume";
            slider.value = GameData.instance.masterVolume;
        }
        else if (audioType == AudioType.music)
        {
            text.text = "Music Volume";
            slider.value = GameData.instance.musicVolume;
        }
        else
        {
            text.text = "Effect Volume";
            slider.value = GameData.instance.sfxVolume;
        }
    }
}
