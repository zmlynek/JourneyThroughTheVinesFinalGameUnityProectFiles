using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OwlNPC : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] Transform startingPos;
    [SerializeField] Transform movePoint;
    PuzzleItems puzzleItems;

    private bool hasTalkedToOwl = false;
    private bool puzzleCompleted = false;
    private bool isCheckingforKills = false;
    private int enemiesDefeated = 1;

    private Animator owlAnimator;

    [SerializeField] List<Enemy> enemies = new List<Enemy>();
    
    void Start()
    {
        owlAnimator = GetComponent<Animator>();
        var pz = FindFirstObjectByType<PuzzleItems>(FindObjectsInactive.Include);
        if (pz != null)
        {
            puzzleItems = pz;
        }
        else Debug.Log("Puzzle items not found");

        movePoint.parent = null;
        transform.position = startingPos.position;
    }


    public void Update()
    {
        //Fly in from top once cutscene activated?
        if (transform.position != movePoint.position)
        {
            owlAnimator.SetBool("isMoving", true);
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, 5 * Time.deltaTime);
        }
        else
        {
            owlAnimator.SetBool("isMoving", false);
        }

        if (SceneManager.GetActiveScene().buildIndex == 3) //only in flower meadow level 
        {
            //check for enemy kills?
            if (!puzzleCompleted && !isCheckingforKills)
            {
                StartCoroutine(CheckForKills());
            }
        }
    }

    IEnumerator CheckForKills()
    {
        //enable lock
        isCheckingforKills = true;

        //update enemy list 
        if (enemies != null)
        {
            foreach (var enemy in enemies.ToList()) //find which enemies have been defeated and remove from list accordingly
            {
                if (!enemy.gameObject.activeSelf) //if enemy game object is disabled (enemy defeated)
                {
                    enemies.Remove(enemy);
                }
            }
            //set enemiesdefeated to list count (how many are left) 
            enemiesDefeated = enemies.Count;

            //if all enemies defeated
            if (enemiesDefeated == 0)
            {
                puzzleCompleted = true;
            }

            //wait before checking again
            yield return new WaitForSeconds(1f);

            //remove lock
            isCheckingforKills = false;
        }
        else
        {
            Debug.Log("enemies is null");
            yield return new WaitForEndOfFrame();
        }        
    }

    public void Interact()
    {
        if (SceneManager.GetActiveScene().buildIndex == 3) //flower meadow dialogue
        {
            if (!hasTalkedToOwl)
            {
                StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
                hasTalkedToOwl = true;
            }
            else //has talked to owl
            {
                if (!puzzleCompleted)
                {
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Owl: Come back once you have taken care of all the enemies!"));
                }
                else if (!puzzleItems.puzzleItems["Relic Piece 1"])//has completed puzzle
                {
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Owl: Well I think that definitely proves you are worthy! Here's the first piece of the relic, Hoohoo! I'll find you again once I've found some more fragments!"));
                    puzzleItems.GiveRelicPiece1();
                }
                else
                {
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("I already gave you the relic piece! I'll find you again when I find more, Hoo Hoo!"));
                }
            }
        }
        else if (SceneManager.GetActiveScene().buildIndex == 6) //shoreside dialogue
        {
            if (!hasTalkedToOwl)
            {
                Debug.Log(hasTalkedToOwl);
                StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
                hasTalkedToOwl = true;
                puzzleItems.GiveRelicPiece2(); 
            }
            else
            {
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("I already gave you the relic piece! I'll find you again when I find more, Hoo Hoo!"));
            }
        }
        else if (SceneManager.GetActiveScene().buildIndex == 8) //ancient ruins dialogue
        {
            if (!hasTalkedToOwl)
            {
                StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
                hasTalkedToOwl = true;
                puzzleItems.GiveRelicPiece3();
            }
            else
            {
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("I already gave you the relic piece! I'll find you again when I find more, Hoo Hoo!"));
            }
        }
    }
}
