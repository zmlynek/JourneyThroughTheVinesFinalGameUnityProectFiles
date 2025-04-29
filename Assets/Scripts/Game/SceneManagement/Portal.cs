using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable 
{
    [SerializeField] int SceneToLoad = -1; //intialize with -1 so if not set in unity causes error instead of going to wrong scene
    [SerializeField] string destination; //initialize in unity to match name of game object with the entrance portal of the scene to load
    [SerializeField] Transform spawnpoint;

    private bool portalLock = false;
    private bool cavesScene = false;
    private bool updateLock = false;

    PlayerController playerController;
    PuzzleItems puzzleItems;
    public event Action OnLevelStart;
    public event Action OnSceneLoaded;

    public void OnPlayerTriggered(PlayerController player)
    {
        //Switch Scenes
        playerController = player;
        StartCoroutine(SwitchScene());
    }
    Fader fader;
    private void Start()
    {
        portalLock = false;
        fader = FindFirstObjectByType<Fader>();
        if (fader == null) { Debug.Log("Fader not found");  }

        if (gameObject.name.CompareTo("PortalToCatCastleLevel") == 0) { cavesScene = true; puzzleItems = FindFirstObjectByType<PuzzleItems>(FindObjectsInactive.Include); }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 10 && !updateLock)
            StartCoroutine(UpdatePortalLock());
    }

    IEnumerator UpdatePortalLock()
    {
        Debug.Log("Setting portal lock false");
        updateLock = true;
        portalLock = false;
        yield return new WaitForSeconds(5f);
        updateLock = false;
    }

    IEnumerator SwitchScene()
    {
        //Debug.Log(cavesScene);
        if (cavesScene) //special case for specific level "caves" 
        {
            if (puzzleItems != null) 
            { 
                if (puzzleItems.puzzleItems["Ranch Key"] && puzzleItems.puzzleItems["River Key"] && puzzleItems.puzzleItems["Snowy Key"] && puzzleItems.puzzleItems["Bio Key"] && puzzleItems.puzzleItems["Cave Key"])
                {
                    yield return LoadNextLevel();
                    cavesScene = false;
                }
            }
            yield return DialogueManager.Instance.TypeSingleLineDialogue("You need more keys to open this!");
            yield return new WaitForSeconds(1f);
        }
        else //regular scene switch
        {
            yield return LoadNextLevel();
        }       
    }

    IEnumerator LoadNextLevel()
    {
        DontDestroyOnLoad(gameObject);
        yield return fader.FadeIn(0.5f);
        yield return SceneManager.LoadSceneAsync(SceneToLoad);
        if (!portalLock)
        {
            OnLevelStart();
            portalLock = true; //when moved to next scene, should prevent the portal that sent you from trying to send you again in the next scene.
            var desPortal = FindObjectsByType<Portal>(FindObjectsSortMode.InstanceID).First(x => x != this && x.name.CompareTo(destination) == 0);
            if (desPortal != null)
            {
                playerController.SetPositionAndSnapToTile(desPortal.SpawnPoint.position);
                playerController.playerCheckPoint.position = desPortal.SpawnPoint.position;
                playerController.LastTileVisited = desPortal.transform.position;

                var companion = FindFirstObjectByType<CompanionController>();
                if (companion != null)
                {
                    companion.SetPositionAndSnapToTile(desPortal.transform.position);
                }
                else Debug.Log("Companion not found");
                playerController.takingPortal = false;
            }
            else Debug.Log("destination portal not found");
            yield return new WaitForEndOfFrame();
        }
        else { Debug.Log("Cannot use this portal right now"); }
        yield return fader.FadeOut(0.5f);
        OnSceneLoaded();

        var gameMusic = FindFirstObjectByType<GameMusic>();
        if (gameMusic != null)
        {
            gameMusic.UpdateMusic();
        }

        Destroy(gameObject);
    }
    public Transform SpawnPoint => spawnpoint;
}
