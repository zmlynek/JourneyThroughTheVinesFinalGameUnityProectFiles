using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLock : MonoBehaviour
{
    GameController gameController;
    [SerializeField] Dialogue dialogue;
    [SerializeField] int targetScene;
    private void Awake()
    {
        gameController = FindFirstObjectByType<GameController>();
    }

    public void CheckLevelLock() //NOTE: logic currently require singleLoadObjects to be in the same scene to determine level visit status ; do NOT put as child of singleLoadObjects
    { //needs logic review
        if (gameController != null)
        {
            targetScene -= 2; //adjust to act as LevelsVisited index

            if (gameController.LevelsVisited != null) //only enter if levelsVisited has been instantiated
            {
                if (gameController.LevelsVisited[targetScene]) //if scene has been loaded before 
                {
                    StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
                    Destroy(gameObject);
                }
                else
                {
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("You have not yet unlocked this level, try again after completing more of the game!"));
                }
            }
        }
    }

}
