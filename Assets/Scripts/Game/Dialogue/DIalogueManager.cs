using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//Class to display dialogue from Dialgoue ; is used by referencing DialogueManager.Instance()
public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialogueBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialogue;
    public event Action OnShowSingleLineDialogue;
    public event Action OnCloseDialogue;

    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    Dialogue dialogue;
    int currentLine = 0;
    bool isTyping;

    public IEnumerator ShowDialogue(Dialogue dialogue)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialogue?.Invoke();

        this.dialogue = dialogue;
        dialogueBox.SetActive(true); //cannot access when on a scene have been to before ; probably has to do with the "Instance" variable
        StartCoroutine(TypeDialogue(dialogue.Lines[0]));
    }

    public IEnumerator TypeDialogue(string dialog)
    {
        isTyping = true;
        dialogText.text = "";
        dialogText.color = Color.black;
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }

    public IEnumerator TypeSingleLineDialogue(string dialog)
    {
        yield return new WaitForEndOfFrame();
        isTyping = true;
        OnShowSingleLineDialogue?.Invoke();
        dialogueBox.SetActive(true);

        dialogText.text = "";
        dialogText.color = Color.black;
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isTyping)
        {
            ++currentLine;
            if (currentLine < dialogue.Lines.Count) //Continue Dialog when done typing
            {
                StartCoroutine(TypeDialogue(dialogue.Lines[currentLine]));
            }
            else //End of dialogue
            {
                currentLine = 0;
                dialogueBox.SetActive(false);
                OnCloseDialogue?.Invoke();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Z) && isTyping)
        {
            lettersPerSecond = 90;
        }
    }

    public void HandleSingleLineDialogue()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isTyping)
        {
            dialogueBox.SetActive(false);
            OnCloseDialogue?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Z) && isTyping)
        {
            lettersPerSecond = 90;
        }
    }
}
