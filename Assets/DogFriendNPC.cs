using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DogFriendNPC : MonoBehaviour, Interactable
{
    [SerializeField] string destination;
    [SerializeField] int sceneToLoad;
    PlayerController playerController;
    GameMusic gameMusic;
    Fader fader;
    public void Interact()
    {
        gameMusic = FindFirstObjectByType<GameMusic>();
        fader = FindFirstObjectByType<Fader>();
        playerController = FindFirstObjectByType<PlayerController>();

        gameMusic.SetClip(0);

        StartCoroutine(RescueAndGoHome());
    }

    IEnumerator RescueAndGoHome()
    {
        DontDestroyOnLoad(gameObject);

        yield return fader.FadeIn(0.5f);
        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        var desPortal = FindObjectsByType<Portal>(FindObjectsSortMode.InstanceID).First(x => x.name.CompareTo(destination) == 0);
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
        yield return fader.FadeOut(0.5f);

        var gameMusic = FindFirstObjectByType<GameMusic>();
        if (gameMusic != null)
        {
            gameMusic.UpdateMusic();
        }
        var dogSpawn = FindObjectsByType<GameObject>(FindObjectsSortMode.InstanceID).First(x => x.name.CompareTo("DogSpawnPoint") == 0);
        if (dogSpawn != null) { transform.position = dogSpawn.transform.position;  gameObject.layer = 8; }
    }
}
