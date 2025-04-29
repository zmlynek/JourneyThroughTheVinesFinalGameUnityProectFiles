using System.Collections.Generic;
using UnityEngine;

public class HorsePuzzleController : MonoBehaviour
{
    private bool puzzleActive = false;
    private int HorsesTalkedTo = 0;
    private bool puzzleCompleted = false;

    [SerializeField] List<string> horseDialogues = new List<string>();
    [SerializeField] List<HorseNPC> horses = new List<HorseNPC>();

    [SerializeField] SheriffNPC sheriffNPC;

    private void Update()
    {
        if (puzzleActive)
        {
            if(HorsesTalkedTo >= 4)
            {
                if (!puzzleCompleted) //make sure line only runs once
                {
                    sheriffNPC.CompletePuzzle();
                    puzzleCompleted = true;
                }
            }
        }
    }

    public void TalkToHorse(HorseNPC currentHorse)
    {
        if (sheriffNPC.hasTalkedToSheriff)
        {
            var randomLine = Mathf.FloorToInt(Random.value * (horseDialogues.Count)); //return random index within horseDialogues
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue(horseDialogues[randomLine]));
            if (!currentHorse.hasBeentalkedTo)
            {
                HorsesTalkedTo++;
                currentHorse.hasBeentalkedTo = true;
            }
                
        }
        else
        {
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("You look new 'round here, you should probably talk to the sheriff first!"));
        }
    }

    public void StartPuzzle()
    {
        puzzleActive = true;
    }
}
