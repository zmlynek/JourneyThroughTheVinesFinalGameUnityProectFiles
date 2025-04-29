using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

public class SingleLoadObjects :MonoBehaviour
{
    public int instanceID;
    private GameController gameController;

    private void Awake() 
    {
        instanceID = GetInstanceID();
        CheckSceneObjects();
    }

    public void CheckSceneObjects()
    {
        gameController = FindFirstObjectByType<GameController>();
        if (gameController != null)
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            currentScene -= 2;

            if (gameController.LevelsVisited != null) //only enter if levelsVisited has been instantiated
            {
                if (gameController.LevelsVisited[currentScene]) //if scene has not been loaded before 
                {
                    Debug.Log("Destroying SceneObjects " + instanceID);
                    Destroy(gameObject);
                }
                else gameController.LevelsVisited[currentScene] = true;    
            }
        }
        else Debug.Log("Could not find game controller");
           
    }
}
