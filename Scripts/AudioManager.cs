using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;
    float masterVolume = 1f;
    float musicVolume = 1f;
    float sfxVolume = 1f;

    private void Start()
    {
        UpdateVolumeLevels();
        if (musicSource.clip != null)
        {
            PlayMusic(musicSource.clip, true);
        }
    }

    public void UpdateVolumeLevels()
    {
        masterVolume = GameData.instance.masterVolume;
        musicVolume = GameData.instance.musicVolume;
        sfxVolume = GameData.instance.sfxVolume;
        musicSource.volume = masterVolume * musicVolume;
    }

    public void PlayMusic(AudioClip music, bool loop = true)
    {
        musicSource.clip = music;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
