using System.Collections;
using UnityEngine;

public class HorseNPC : MonoBehaviour, Interactable
{
    [SerializeField] Transform movePoint;
    public LayerMask whatStopsMovement;
    public bool hasBeentalkedTo = false;
    public void Interact()
    {
        var puzzleController = GetComponentInParent<HorsePuzzleController>();
        if (puzzleController != null)
        {
            puzzleController.TalkToHorse(this);
        }
    }
}
