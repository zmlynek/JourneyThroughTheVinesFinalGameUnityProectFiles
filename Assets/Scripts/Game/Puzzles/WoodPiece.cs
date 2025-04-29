using System.Collections;
using UnityEngine;

public class WoodPiece : MonoBehaviour, Interactable
{
    LoneVillagerNPC loneVillager;

    void Awake()
    {
        var LoneVillager = FindAnyObjectByType<LoneVillagerNPC>();
        if (LoneVillager != null)
        {
            loneVillager = LoneVillager;
        }
        else Debug.Log("Could not find lone villager npc");
    }

    public void Interact()
    {
        if (!loneVillager.puzzleStarted)
        {
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Companion: Hm, seems to be a piece of wood. Wonder what they're for... maybe you should ask that villager?"));
        }
        if (loneVillager.puzzleStarted)
        {
            StartCoroutine(PickUpWood());
        }
    }

    IEnumerator PickUpWood()
    {
        yield return (DialogueManager.Instance.TypeSingleLineDialogue("You picked up a piece of wood!"));
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    }
}
