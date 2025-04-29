using System;
using UnityEngine;

//Class to hold relevant details for each cutscene i.e. duration, dialogue, camera position, etc. and all relevant methods/references 
public class Cutscene : MonoBehaviour
{
    [SerializeField] Dialogue dialogue;
    //[SerializeField] Transform cameraPos;
    private Transform companionMovePoint;
    private Transform playerMovePoint;

    private Animator playerAnim;

    private bool hasBeenTriggered = false;

    public event Action OnCloseCutscene;

    private void Start()
    {
        var companion = FindFirstObjectByType<CompanionController>();
        if (companion != null)
        {
            companionMovePoint = companion.movePoint;
        }
        else Debug.Log("Companion not found");
        var player = FindFirstObjectByType<PlayerController>();
        if (companion != null)
        {
            playerMovePoint = player.movePoint;
            playerAnim = player.gameObject.GetComponent<Animator>();
        }
        else Debug.Log("Player not found");
    }

    //method to move one tile at a time towards a location? 

    //method to start the cutscene and show dialog
    public void StartCutscene()
    {
        if (!hasBeenTriggered) //only trigger once
        {
            var dx = Mathf.FloorToInt(playerMovePoint.position.x) - Mathf.FloorToInt(companionMovePoint.position.x);
            var dy = Mathf.FloorToInt(playerMovePoint.position.y) - Mathf.FloorToInt(companionMovePoint.position.y);
            if (dx == 1) //companion to left
            {
                playerAnim.SetFloat("moveX", -1);
                playerAnim.SetFloat("moveY", 0);
            }
            else if (dx == -1) //companion to right
            {
                playerAnim.SetFloat("moveX", 1);
                playerAnim.SetFloat("moveY", 0);
            }
            else if (dy == 1) //companion below
            {
                playerAnim.SetFloat("moveX", 0);
                playerAnim.SetFloat("moveY", -1);
            }
            else if (dy == -1) //companion above
            {
                playerAnim.SetFloat("moveX", 0);
                playerAnim.SetFloat("moveY", 1);
            }

            StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
            OnCloseCutscene();
            hasBeenTriggered = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
