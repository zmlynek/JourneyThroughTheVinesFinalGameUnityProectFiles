using UnityEngine;

public class VillageChildNPC : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue dialogue;
    private PuzzleItems puzzleItems;

    private void Awake()
    {
        puzzleItems = FindFirstObjectByType<PuzzleItems>();
    }

    public void Interact()
    {
        StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
        puzzleItems.GiveBioKey();
    }
}
