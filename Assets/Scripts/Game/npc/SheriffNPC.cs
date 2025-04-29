using UnityEngine;

public class SheriffNPC : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue dialogue; //hold first conversation before giving key ; giving key uses singleLineDialogue
    [SerializeField] HorsePuzzleController puzzleController;
    private PuzzleItems puzzleItems;
    private bool hasGivenKey = false;
    public bool hasTalkedToSheriff = false; 
    private bool puzzleCompleted = false;

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
        if (!hasTalkedToSheriff)
        {
            StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
            hasTalkedToSheriff = true; puzzleController.StartPuzzle();
        }
        else //has talked to sheriff
        { 
            if (!hasGivenKey && !puzzleCompleted)
            {
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Sheriff: You might wanna talk to all the horses first!"));
            }
            else if (!hasGivenKey && puzzleCompleted)
            {
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Sheriff: Well I'll be... Here's your key partner!"));

                //logic to give key as item and add to player's items
                puzzleItems.GiveRanchKey();

                hasGivenKey = true;

            }
            else if (hasGivenKey && puzzleCompleted)
            {
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Sheriff: You already got the key partner."));
            }

        }
    }
    public void CompletePuzzle()
    {
        puzzleCompleted = true;
    }
}
