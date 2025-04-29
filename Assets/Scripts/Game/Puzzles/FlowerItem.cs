using System.Collections;
using UnityEngine;

public class FlowerItem : MonoBehaviour, Interactable
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
            StartCoroutine(PickupFlower());
        }
        else Debug.Log("Puzzle items not found ; could not pickup flower");
    }

    IEnumerator PickupFlower()
    {
        //using coroutine to wait for dialogue before removing the flower from screen and adding to player's menu
        yield return StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
        puzzleItems.GiveFlowerItem();
        gameObject.SetActive(false);
    }
}
