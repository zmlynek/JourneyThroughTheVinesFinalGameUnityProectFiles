using UnityEngine;

public class MayorNPC : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue dialogue;
    PuzzleItems puzzleItems;
    GameController gameController;

    private bool hasTalkedToNPC = false;
    private bool hasGivenKey = false;
    private bool puzzleCompleted = false;

    private void Awake()
    {
        var gameController = GetComponent<GameController>();
        if (gameController != null)
        {
            this.gameController = gameController;
        }
        else { Debug.Log("Could not find game controller"); }

        var puzzleItems = FindFirstObjectByType<PuzzleItems>(FindObjectsInactive.Include);
        if (puzzleItems != null)
        {
            this.puzzleItems = puzzleItems;
            if (this.puzzleItems.puzzleItems["Flower"]) { puzzleCompleted = true; }
        }
        else Debug.Log("Could not find Puzzle Items");
    }

    // need to implement a save feature to make this work properly.

    public void Interact()
    {
        if (!hasTalkedToNPC) 
        {
            StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
            hasTalkedToNPC = true; 
        }
        else
        {
            if (!hasGivenKey && !puzzleCompleted)
            {
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Please help us find the flower!"));
            }
            else if (!hasGivenKey && puzzleCompleted)
            {
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Thanks for your help! Here, hopefully this key is helpful to you!"));
                if (puzzleItems != null)
                {
                    puzzleItems.GiveSnowyKey();
                }
            }
            else if (hasGivenKey)
            {
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("I already gave you the key, thanks again for your help!"));
            }
        }
    }
}
