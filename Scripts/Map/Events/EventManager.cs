using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventManager : MonoBehaviour
{
    public MapManager mapManager;
    public GameObject eventContainer;

    [Header("Description")]
    public Image descriptionImage;
    public TextMeshProUGUI description;

    [Header("EventButtons")]
    public GameObject buttonContainer;
    public EventButton[] eventButtons;

    public void StartEvent(Event islandEvent)
    {
        // Combat event
        if (islandEvent.links.Length == 0 && islandEvent.combats.Length > 0)
        {
            GameData.instance.combatDifficulty = mapManager.currentSector.GetCombatDifficulty();
            StartCoroutine(StartCombat(islandEvent));
            return;
        }

        mapManager.StartingEvent();
        eventContainer.SetActive(true);
        descriptionImage.sprite = islandEvent.icon;
        description.text = islandEvent.description;

        for(int i = 0; i < eventButtons.Length; i++)
        {
            if (islandEvent.links.Length > i && islandEvent.links[i] != null)
            {
                eventButtons[i].gameObject.SetActive(true);
                eventButtons[i].SetEvent(islandEvent.links[i]);
            }
            else
            {
                eventButtons[i].gameObject.SetActive(false);
            }
        }
        GameData.instance.data.eventName = islandEvent.name;
    }

    public void EventButtonClicked(int index)
    {
        Event newEvent = eventButtons[index].islandEvent;
        
        // Aplly effects
        foreach(EventEffect effect in newEvent.effects)
        {
            Effect(effect);
        }

        // Start combat
        if (newEvent.combats != null && newEvent.combats.Length > 0)
        {
            StartCoroutine(StartCombat(newEvent));
        }

        // More options ahead
        else if (newEvent.links.Length > 0)
        {
            StartEvent(newEvent);
        }

        // End of event chain
        else
        {
            eventContainer.SetActive(false);
            mapManager.EndOfEvent();
        }
    }

    private IEnumerator StartCombat(Event newEvent)
    {
        Debug.Log("Starting combat");
        mapManager.StartingCombat();
        yield return new WaitForSeconds(mapManager.combatStartDelay);

        Debug.Log("Go Combat!");
        GameData.instance.data.eventName = newEvent.links.Length == 0 ? "none" : newEvent.name;
        Combat combat = newEvent.combats[Random.Range(0, newEvent.combats.Length)];
        //GameData.instance.monsters = new List<Unit>(combat.monsters);
        GameData.instance.combat = combat;
        GameData.instance.LoadScene("CombatScene");
    }

    public void Effect(EventEffect effect)
    {
        switch (effect.reward)
        {
            case Reward.Resource:
                {
                    int current = GameData.instance.data.resources;
                    if (effect.value < 0)
                    {
                        current += effect.value;
                        GameData.instance.data.resources = Mathf.Max(current, 0);
                    }
                    else
                    {
                        current += effect.value;
                        GameData.instance.data.resources = Mathf.Min(current, GameData.instance.data.maxResources);
                    }
                    mapManager.mapUI.UpdateResources();
                }
                break;

            case Reward.MaxResource:
                {
                    int current = GameData.instance.data.maxResources;
                    if (effect.value < 0)
                    {
                        current += effect.value;
                        GameData.instance.data.maxResources += Mathf.Max(current, 0);
                    }
                    else
                    {
                        GameData.instance.data.maxResources += effect.value;
                    }
                    mapManager.mapUI.UpdateResources();
                }
                break;

            case Reward.Coin:
                {
                    int current = GameData.instance.data.coins + effect.value;
                    GameData.instance.data.coins = Mathf.Max(current, 0);
                    mapManager.mapUI.UpdateCoin();
                }
                break;
        }
    }

    
}
