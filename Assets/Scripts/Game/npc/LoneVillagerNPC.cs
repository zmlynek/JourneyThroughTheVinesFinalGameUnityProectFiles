using UnityEngine;

public class LoneVillagerNPC : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] GameObject bridgeCover; 
    private PuzzleItems puzzleItems;
    private bool hasTalkedtoNPC = false;
    public bool puzzleStarted = false;
    private bool puzzleFinished = false;
    private bool hasGivenKey = false;

    private void Awake()
    {
        var puzzleItems = FindFirstObjectByType<PuzzleItems>(FindObjectsInactive.Include);
        if (puzzleItems != null)
        {
            this.puzzleItems = puzzleItems;
        }
        else Debug.Log("Could not find Puzzle Items");
    }

    public void Interact()
    {
        if (!hasTalkedtoNPC)
        {
            StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
            hasTalkedtoNPC = true; puzzleStarted = true;
        }
        else
        {
            if (puzzleStarted && !puzzleFinished)
            {
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Lone Villager: Come talk to me again after you have picked up all that wood!"));
            }
            else if (puzzleFinished && !hasGivenKey)
            {
                Destroy(bridgeCover);
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Lone Villager: Great, the bridge is fixed now! As per our deal, here's your key!"));

                puzzleItems.GiveRiverKey();

                hasGivenKey = true;
            }
            else if (puzzleFinished && hasGivenKey)
            {
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Lone Villager: Thanks again for your help, and good luck with the rescue!"));
            }
        }
    }

    public void FinishPuzzle()
    {
        puzzleFinished = true;
    }
}
