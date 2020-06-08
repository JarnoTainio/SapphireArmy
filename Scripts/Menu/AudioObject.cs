using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClip;

    [Header("Settings")]
    public bool randomSound;

    int index;

    public void Play()
    {
        if (randomSound)
        {
            index = Random.Range(0, audioClip.Length);
        }
        else
        {
            index++;
            if (index >= audioClip.Length)
            {
                index = 0;
            }
        }
        audioSource.clip = audioClip[index];
        audioSource.volume = GameData.instance.masterVolume * GameData.instance.sfxVolume;
        audioSource.Play();
    }
}

public enum AudioType { master, music, sfx}