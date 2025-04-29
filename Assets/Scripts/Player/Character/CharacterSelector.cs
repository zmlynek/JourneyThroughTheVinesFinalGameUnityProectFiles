using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using System.Collections;

//This class is used to execute methods for buttons on the character selection menu
public class CharacterSelector : MonoBehaviour
{
    public SpriteRenderer sr;
    public List<Sprite> characters = new List<Sprite>();
    public List<TextMeshProUGUI> characterTexts = new List<TextMeshProUGUI>();
    //list of classes ? 
    private int currentCharacter = 0; //0 = swordsman, 1 = mage, 2 = rogue, 3 = archer
    private TextMeshProUGUI currentText;
    public GameObject characterChoice;

    //better logic: based on skin show related text object and update player prefab with correct skin and animator
    private void Start()
    {
        currentText = characterTexts[currentCharacter];
    }

    public void NextOption()
    {
        currentText = characterTexts[currentCharacter]; //text field to hide
        currentCharacter++;
        if (currentCharacter == characters.Count)
        {
            currentCharacter = 0; //index of text field to show
        }

        currentText.gameObject.SetActive(false);
        currentText = characterTexts[currentCharacter];
        characterTexts[currentCharacter].gameObject.SetActive(true);
        sr.sprite = characters[currentCharacter];
    }

    public void BackOption()
    {
        currentText = characterTexts[currentCharacter]; //text field to hide
        currentCharacter--;
        if (currentCharacter < 0)
        {
            currentCharacter = characters.Count - 1; //index of text field to show
        }

        currentText.gameObject.SetActive(false);
        currentText = characterTexts[currentCharacter];
        characterTexts[currentCharacter].gameObject.SetActive(true);
        sr.sprite = characters[currentCharacter];
    }

    public void PlayGame() //IMPORTANT: add logic to set player to *correct prefab* (currently keeps stats if game is not closed)
    {
        StartCoroutine(SwitchToHomeLevel());
    }

    IEnumerator SwitchToHomeLevel()
    {
        DontDestroyOnLoad(gameObject);
        yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        //update character.classType appropriately
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            var character = FindFirstObjectByType<Character>();
            if (character != null)
            {
                character.classType = currentText.text;
            }
            else Debug.Log("Character Selector: Character not found");
            yield return new WaitForEndOfFrame();
        }
        else Debug.Log("Could not access correct scene");
        Destroy(gameObject);
    }

}
