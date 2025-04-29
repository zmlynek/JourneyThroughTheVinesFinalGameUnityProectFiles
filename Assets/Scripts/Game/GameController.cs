using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public enum GameState { GameStart, LevelStart, FreeRoam, Dialogue, SingleLineDialogue, Battle, BossBattle, MapMenu, UpgradesMenu, ItemsMenu, Cutscene, Busy}

//This class is the most important as it handles each state of the game, allowing each class to have control at the proper time, and organizing the order in which events happen 
//Added to invisible game object that is called GameController
public class GameController : MonoBehaviour
{
    [SerializeField] BattleSystem battleSystem; 
    [SerializeField] PlayerController playerController;
    [SerializeField] CompanionController companion;
    [SerializeField] Camera worldCam;
    [SerializeField] Transform checkpoint;
    [SerializeField] SkillTree skillTree;

    [SerializeField] UIHandler uiHandler;
    [SerializeField] GameObject gameUI; //to set visible or not
    public MenuButtons menuUI; //to check for event
    public MenuButtons backUI; //to check for event

    //prefabs to control which character was chosen from character selection
    public GameObject selectedCharacter;
    public GameObject player;
    private Boss finalBoss;

    //level completions
    public bool[] LevelCompletions {  get; private set; }
    public bool[] LevelsVisited { get; private set; }

    private Sprite playerSprite; //correct sprite chosen from character selection
    //need reference to correct animator based on character selection

    GameState state = GameState.GameStart; //Initial game state

    Portal Portal { get; set; }

    [SerializeField] List<Enemy> enemyList = new List<Enemy>();
    private Enemy CurrentEnemy { get; set; }

    [SerializeField] List<Cutscene> cutscenes = new List<Cutscene>();
    private Cutscene CurrentCutscene { get; set; }

    //mutex boolean 
    private bool timerLock = false;
    private bool inMenu = false;
    private bool inBattle = false;


    private SingleLoadObjects[] SceneObjects;
    public void Start()
    {
        LevelCompletions = new bool[10];
        LevelsVisited = new bool[10];

        //uiDocument = gameUI.GetComponent<UIDocument>();
        playerSprite = selectedCharacter.GetComponent<SpriteRenderer>().sprite;
        player.GetComponent<SpriteRenderer>().sprite = playerSprite;

        playerController.OnEncounter += (Collider2D enemyCollider) =>
        {
            if (enemyCollider.TryGetComponent<Enemy>(out var enemy))
            {
                CurrentEnemy = enemy;
                inBattle = true;
                StartBattle(enemy);
            }
        };

        battleSystem.OnBattleOver += EndBattle;
        battleSystem.OnBossBattleOver += EndBossBattle;

        playerController.OnCutscene += (Collider2D cutsceneCollider) =>
        {
            var cutscene = cutsceneCollider.GetComponentInParent<Cutscene>();
            if (cutscene != null)
            {
                state = GameState.Cutscene;
                //find which cutscene it is
                for (int i = 0; i < cutscenes.Count; i++)
                {
                    if (cutscene == cutscenes[i])
                    {
                        cutscenes[i].StartCutscene();
                        CurrentCutscene = cutscenes[i];
                    }
                }
            }
        }; ;

        playerController.OnPortal += (Collider2D portalCollider) =>
        {
            Portal = portalCollider.GetComponentInParent<Portal>();
            Portal.OnSceneLoaded += SceneLoaded;
            Portal.OnLevelStart += StartLevel;
            TriggerPortal(Portal);
        };

        DialogueManager.Instance.OnShowDialogue += () =>
        {
            state = GameState.Dialogue;
        };

        DialogueManager.Instance.OnCloseDialogue += () =>
        {
            if (!inMenu && !inBattle)
            {
                state = GameState.FreeRoam;
            }
            else if (inMenu && !inBattle)
            {
                state = GameState.MapMenu;
            }
            else if (!inMenu && inBattle)
            {
                state = GameState.Battle;
            }
        };

        DialogueManager.Instance.OnShowSingleLineDialogue += () =>
        {
            state = GameState.SingleLineDialogue;
        };

        skillTree.OnFailedUpgrade += FailedUpgrade;

        foreach (var c in cutscenes)
        {
            c.OnCloseCutscene += CloseCutscene;
        }

        menuUI.OnEnterMenu += () =>
        {
            inMenu = true;
            state = GameState.MapMenu;
        };
        backUI.OnExitMenu += () =>
        {
            inMenu = false;
            LoadUIStats();
            state = GameState.FreeRoam;
        };

        playerController.StartBossFight += StartBossBattle;
    }

    void LoadUIStats()
    {
        //set UI values to player values
        uiHandler.UpdateMaxHP(playerController.PlayerChar.Health);
        uiHandler.UpdateHP(playerController.PlayerChar.HP);
        uiHandler.UpdateDEF(playerController.PlayerChar.Defense);
        uiHandler.UpdateATK(playerController.PlayerChar.Attack);
        uiHandler.UpdateCompanionMaxHP(companion.MaxHealth);
        uiHandler.UpdateCompanionHP(companion.HP);
        uiHandler.UpdateCompanionATKValue(companion.Attack);
        uiHandler.UpdateCompanionDEFValue(companion.Defense);
        uiHandler.UpdateClassAbilityCooldown(playerController.PlayerChar.Abilities[0].Cooldown);
        uiHandler.UpdateHealAbilityCooldown(playerController.PlayerChar.Abilities[1].Cooldown);

        playerController.PlayerChar.SetPlayerHPBar((float)playerController.PlayerChar.HP / playerController.PlayerChar.Health);
        companion.SetCompanionHPBar((float)companion.HP / companion.MaxHealth);
    }

    void CompanionStartGame() //Starting 'cutscene' ; game opens with dialogue not entered by player
    {
        //StartCoroutine(companion.OnStartGame());
        companion.GetComponent<Interactable>()?.Interact();
    }

    void StartBattle(Enemy enemy)
    {
        state = GameState.Battle;

        battleSystem.StartBattle(enemy);
    }
    void EndBattle(bool won) //called from battle system ; returns the boolean of who won (true if player won)
    {       
        //reset to last checkpoint if lost
        if (!won)
        {
            state = GameState.Busy;
            ResetToLastCheckPoint();
        }
        
        inBattle = false;

        //Return to singleLineDialogue after end of battle execution before returning to freeroam
        state = GameState.SingleLineDialogue;
    }

    void EndBossBattle(bool won)
    {
        if (!won)
        {
            state = GameState.Busy;
            ResetToLastCheckPoint();
        }
        inBattle = false;

        state = GameState.FreeRoam;
    }

    void FailedUpgrade()
    {
        state = GameState.SingleLineDialogue;
    }

    void CloseCutscene()
    {
        //have to be in freeroam to trigger cutscene so return player to freeroam
        state = GameState.FreeRoam;
    }

    void TriggerPortal(Portal portal)
    {
        state = GameState.Busy;
        //logic for which portal to open should go here
        portal.OnPlayerTriggered(playerController); 
        enemyList.Clear();
    }

    void StartLevel()
    {
        state = GameState.LevelStart;
    }

    void SceneLoaded()
    {
        state = GameState.FreeRoam;
    }

    public void UpdateEnemyList(List<Enemy> enemies)
    {
        enemyList = enemies;
    }

    public void UpdateCutsceneList(List<Cutscene> cutscenes)
    {
        this.cutscenes = cutscenes;
        foreach (var c in this.cutscenes)
        {
            c.OnCloseCutscene += CloseCutscene;
        }
    }

    void ResetToLastCheckPoint()
    {
        //set player and movepoint positions
        StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("You ran too low on health and had to rest!"));
        playerController.SetPositionAndSnapToTile(checkpoint.position);
        playerController.ResetHealth();

        //also set companion and companion movepoint positions
        var tileToRightOfCheckPoint = new Vector3(checkpoint.position.x, checkpoint.position.y - 1);
        playerController.LastTileVisited = tileToRightOfCheckPoint;
        companion.transform.position = tileToRightOfCheckPoint;
        companion.movePoint.position = tileToRightOfCheckPoint;
        companion.ResetHealth(); 

        LoadUIStats();
    }

    void StartBossBattle()
    {
        Debug.Log("Starting Boss battle");
        finalBoss = FindFirstObjectByType<Boss>();
        inBattle = true;
        state = GameState.BossBattle;
        battleSystem.StartBossBattle();
    }

    public void CompleteLevel(int levelIndex)
    {
        LevelCompletions[levelIndex] = true;
    }

    public void VisitLevel(int levelIndex)
    {
        LevelsVisited[levelIndex] = true;
    }

    private void Update() //Handles game states (try to remove as much as possible)
    {
        if (state == GameState.FreeRoam)
        {
            //these statements should only execute once
            if (!gameUI.activeSelf)
                gameUI.SetActive(true);
            if (CurrentEnemy != null)
                CurrentEnemy = null;

            //these statements should execute while in free roam
            playerController.HandleUpdateFreeRoam();
            companion.HandleUpdateFreeRoam();
            

            if (enemyList != null)
            {
                foreach (var enemy in enemyList)
                {
                    if (enemy != null && enemy.isActiveAndEnabled)
                        enemy.HandleUpdate();
                }
            }
            if (!timerLock)
            {
                timerLock = true;
                StartCoroutine(uiHandler.HandleAbilityTimerUpdate());
                timerLock = false;
            }
        }
        else if (state == GameState.GameStart) //On start load UI info and start companion dialogue
        {
            SceneObjects = FindObjectsByType<SingleLoadObjects>(FindObjectsSortMode.None);
            foreach (var sceneObject in SceneObjects)
            {
                sceneObject.CheckSceneObjects();
            }
            LoadUIStats();
            uiHandler.HideMapMenu();
            uiHandler.UpdateClassAbilityImage(playerController.PlayerChar.classType);
            CompanionStartGame();
        }
        else if (state == GameState.LevelStart)
        {
            //this was here for portal logic, ended up not needing it ; maybe useable in the future
        }
        else if (state == GameState.Battle) //Handle all updates during battle  (maybe move to battle system)
        {
            battleSystem.HandleUpdate();
            playerController.HandleUpdateBattle();
            companion.HandleUpdateBattle(CurrentEnemy);
            CurrentEnemy.HandleUpdateBattle();

            if (!timerLock)
            {
                timerLock = true;
                StartCoroutine(uiHandler.HandleAbilityTimerUpdate());
                timerLock = false;
            }
        }
        else if (state == GameState.BossBattle) //Handle all updates during battle  (maybe move to battle system)
        {
            battleSystem.HandleUpdate();
            playerController.HandleUpdateBattle();
            companion.HandleUpdateBattle(finalBoss);

            if (!timerLock)
            {
                timerLock = true;
                StartCoroutine(uiHandler.HandleAbilityTimerUpdate());
                timerLock = false;
            }
        }
        else if (state == GameState.MapMenu) //Handle menu updates then store upgrade/item info
        {
            //update skills / stats with new values from skill tree  ?
            //mapMenu.HandleUpdate();?
        }
        else if (state == GameState.Dialogue) //Let the dialogue play line by line while disallowing the player to move
        {
            //gameUI.SetActive(false);
            DialogueManager.Instance.HandleUpdate();
        }
        else if (state == GameState.SingleLineDialogue)
        {
            DialogueManager.Instance.HandleSingleLineDialogue();
        }
        else if (state == GameState.Cutscene)
        {
            //DialogueManager.Instance.HandleUpdate();
            state = GameState.FreeRoam;
        }
    }
}


