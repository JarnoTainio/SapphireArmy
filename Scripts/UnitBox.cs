using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitBox : MonoBehaviour
{
    public GraveyardManager graveyard;

    [Header("Text")]
    public Image unitImage;
    public TextMeshProUGUI unitName;
    public TextMeshProUGUI life;
    public TextMeshProUGUI energy;
    public TextMeshProUGUI armor;
    public TextMeshProUGUI move;
    public TextMeshProUGUI revive;
    public TextMeshProUGUI reviveButtonText;

    [Header("Background")]
    public Image background;
    public Button reviveButton;
    public Color deadColor;
    public Color revivedColor;

    [Header("Actions & Passives")]
    public ActionBox[] actions;
    public ActionBox[] passives;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip reviveSound;

    [HideInInspector]
    public int reviveCost;
    int index;
    bool isDead;

    public void SetUnit(Unit unit, int index, bool isAlive)
    {

        this.index = index;
        unitImage.sprite = unit.sprite;
        isDead = !isAlive;
        reviveCost = unit.GetReviveCost();

        reviveButton.gameObject.SetActive(!isAlive);

        unitName.text = unit.unitName;
        life.text = unit.GetMaxLife().ToString();
        energy.text = unit.GetMaxEnergy().ToString();
        armor.text = unit.GetMaxArmor().ToString();
        move.text = unit.move.ToString();
        revive.text = unit.GetReviveCost().ToString();

        reviveButtonText.text = revive.text;

        for (int i = 0; i < unit.actions.Length; i++)
        {
            Action action = unit.actions[i];
            actions[i].gameObject.SetActive(action != null);
            if (action != null)
            {
                actions[i].Set(action.icon, action.description);
            }
            else
            {
                //actions[i].Set(null, "");
            }
        }
    }

    public void CanBeRevived(bool canBeRevived)
    {

        canBeRevived = canBeRevived && isDead;
        reviveButton.interactable = canBeRevived;
        if (isDead)
        {
            background.color = canBeRevived ? deadColor : Color.white;
        }
    }

    public void Revive()
    {
        isDead = false;
        background.color = Color.white; //revivedColor;
        reviveButton.gameObject.SetActive(false);
        
        graveyard.Revive(index);

        audioSource.volume = GameData.instance.masterVolume * GameData.instance.sfxVolume;
        audioSource.PlayOneShot(reviveSound);
    }

}
