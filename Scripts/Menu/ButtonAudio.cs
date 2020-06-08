using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Sounds")]
    public AudioClip clickedSound;
    public AudioClip cantClickSound;
    public AudioClip toggleSound;

    [Header("Options")]
    public bool toggleSounds;

    bool toggle;
    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Clicked()
    {
        if (cantClickSound != null)
        {
            audioSource.PlayOneShot(button.interactable ? clickedSound : cantClickSound);
        }
        else
        {
            if (toggleSound)
            {
                toggle = !toggle;
                audioSource.PlayOneShot(toggle ? clickedSound : toggleSound);
            }
            else
            {
                audioSource.PlayOneShot(clickedSound);
            }
        }
    }

}
