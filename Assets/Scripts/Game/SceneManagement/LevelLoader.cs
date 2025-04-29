using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] string levelName;
    [SerializeField] List<Enemy> enemyList = new List<Enemy>();
    [SerializeField] List<Cutscene> cutscenes = new List<Cutscene>();

    private GameController gameController;

    private void Start()
    {
        gameController = FindFirstObjectByType<GameController>();
        if (gameController != null)
        {
            gameController.UpdateEnemyList(enemyList);
            gameController.UpdateCutsceneList(cutscenes);
        }
        else Debug.Log("GameController not found");
    }

}
