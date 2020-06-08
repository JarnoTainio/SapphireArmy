using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeoutPanel : MonoBehaviour
{
    public Image image;
    public float duration;
    public bool fading;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeOut(string scene)
    {
        fading = true;
        float time = 0f;

        // Enable panel
        image.color = new Color(0, 0, 0, 0f);
        gameObject.SetActive(true);

        // Transition
        while (time < duration)
        {
            time += Time.deltaTime;
            float f = time / duration;
            image.color = new Color(0, 0, 0, f);
            yield return null;
        }

        image.color = new Color(0, 0, 0, 1f);
        fading = false;
    }

    public IEnumerator FadeIn()
    {
        fading = true;
        float time = duration;

        // Enable panel
        image.color = new Color(0, 0, 0, 1f);
        gameObject.SetActive(true);

        // Transition
        while (time > 0)
        {
            time -= Time.deltaTime;
            float f = time / duration;
            image.color = new Color(0, 0, 0, f);
            yield return null;
        }

        // Hide panel
        image.color = new Color(0, 0, 0, 0);
        gameObject.SetActive(false);
        fading = false;
    }
}
