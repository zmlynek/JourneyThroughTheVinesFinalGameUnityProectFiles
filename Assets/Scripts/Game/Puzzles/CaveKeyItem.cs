using System.Collections;
using UnityEngine;

public class CaveKeyItem : MonoBehaviour, Interactable
{
    PuzzleItems puzzleItems;
    [SerializeField] Dialogue dialogue;
    private void Awake()
    {
        var pz = FindFirstObjectByType<PuzzleItems>(FindObjectsInactive.Include);
        if (pz != null)
        {
            puzzleItems = pz;
        }
        else Debug.Log("Could not find puzzleItems");
    }

    public void Interact()
    {
        if (puzzleItems != null)
        {
            StartCoroutine(PickupCaveKey());
        }
        else Debug.Log("Puzzle items not found ; could not pickup key");
    }

    IEnumerator PickupCaveKey()
    {
        //using coroutine to wait for dialogue before removing the flower from screen and adding to player's menu
        yield return StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
        puzzleItems.GiveCaveKey();
        gameObject.SetActive(false);
    }
}
