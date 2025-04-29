using UnityEngine;

//can be added to any npc that has dialogue or cutscene ; used to interact 
public class npcController : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue dialogue;

    public void Interact()
    {
        //Debug.Log("Interacting with NPC");
        StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
    }
}
