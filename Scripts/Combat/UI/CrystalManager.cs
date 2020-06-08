using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrystalManager : MonoBehaviour
{
    public Image[] crystals;
    public Sprite crystalSprite;
    public Sprite activeSprite;
    public Sprite usedSprite;

    int max;
    int current;

    public void SetCrystals(int amount)
    {
        max = current = amount;
        for (int i = 0; i < crystals.Length; i++)
        {
            crystals[i].gameObject.SetActive(i < amount);
        }
    }

    public void RefreshCrystals()
    {
        StartCoroutine(Refresh());
    }

    private IEnumerator Refresh()
    {
        for (int i = crystals.Length - 1; i >= 0; i--)
        {
            Image image = crystals[i];
            image.sprite = crystalSprite;
            if (image.gameObject.activeSelf)
            {
                image.GetComponent<UIEffect>().TargetSize(1.5f, 3);
                yield return new WaitForSeconds(.3f);
            }
        }
        current = max;
    }

    public void UseCrystal()
    {
        current--;
        crystals[current].sprite = usedSprite;
    }
}
