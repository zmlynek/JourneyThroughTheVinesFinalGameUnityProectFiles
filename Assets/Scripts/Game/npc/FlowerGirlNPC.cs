using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlowerGirlNPC : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue dialogue;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        StartCoroutine(FlowerGirlDialogue());
    }

    IEnumerator FlowerGirlDialogue()
    {
        yield return DialogueManager.Instance.ShowDialogue(dialogue);
        //animator.SetBool("isWalking", true);
        yield return new WaitForSeconds(12f);
        for (int i = 0; i < 5; i++) { transform.position += transform.up; yield return new WaitForSeconds(0.3f); } // += new Vector3(transform.postition.x, transform.position.y + 1)
        gameObject.SetActive(false);
    }
}
