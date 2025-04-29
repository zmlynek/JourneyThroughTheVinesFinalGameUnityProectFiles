using DG.Tweening;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private bool isMoving;
    public bool takingPortal;
    public Transform movePoint;
    public Transform playerCheckPoint;
    public LayerMask whatStopsMovement;
    public LayerMask enemies;
    public LayerMask npc;
    public LayerMask cutscene;
    public LayerMask portal;
    public LayerMask levelLock;
    public LayerMask boss;

    public event Action<Collider2D> OnEncounter;
    public event Action<Collider2D> OnCutscene;
    public event Action OnUseHeal;
    public event Action<Collider2D> OnPortal;
    public event Action StartBossFight;

    private Animator animator;
    private Animator companionAnim;
    public Vector3 LastTileVisited {  get; set; }

    [SerializeField] Character playerChar;

    public Character PlayerChar { get { return playerChar; } }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        companionAnim = FindFirstObjectByType<CompanionController>().GetComponent<Animator>();
        takingPortal = false;
    }

    public void ResetHealth()
    {
        playerChar.HP = playerChar.Health; //reset to max 
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.1f, whatStopsMovement | npc | levelLock) != null || Physics2D.OverlapCircle(targetPos, 0.2f, enemies) != null) 
        {
            return false;
        }
        return true;
    }

    private void Start()
    {
        movePoint.parent = null; //detach so it moves seperately from player
        playerCheckPoint.parent = null; //same as above
        SetClassTypeInAnim(playerChar.classType);
        SetPositionAndSnapToTile(transform.position);

        //intial tile positions for companion to follow
        LastTileVisited = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
    }

    private void SetClassTypeInAnim(string classType) 
    {
        if (classType.CompareTo("Swordsman") == 0)
            animator.SetFloat("classType", 1);
        else if (classType.CompareTo("Mage") == 0)
            animator.SetFloat("classType", 2);
        else if (classType.CompareTo("Rogue") == 0)
            animator.SetFloat("classType", 3);
        else if (classType.CompareTo("Archer") == 0)
            animator.SetFloat("classType", 4);
    }

    void OnMoveOver()
    {
        CheckForEnemies(); 
        CheckForBoss();
        CheckForCutscene();
        CheckForPortal();
        CheckForLevelLock();
    }

    void CheckMoveInputs() //Check move inputs and interact inputs as well 
    {
        isMoving = true; //used more for mutual exclusion

        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
        {
            animator.SetFloat("moveX", Input.GetAxisRaw("Horizontal"));
            animator.SetFloat("moveY", 0f);
            companionAnim.SetFloat("moveX", Input.GetAxisRaw("Horizontal"));
            companionAnim.SetFloat("moveY", 0f);
            if (IsWalkable(transform.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f)))
            {
                animator.SetBool("isMoving", isMoving); //only set animator if actually moving
                companionAnim.SetBool("isMoving", isMoving);
                LastTileVisited = transform.position;
                movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
            }
        }
        else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
        {
            animator.SetFloat("moveX", 0f);
            animator.SetFloat("moveY", Input.GetAxisRaw("Vertical"));
            companionAnim.SetFloat("moveX", 0f);
            companionAnim.SetFloat("moveY", Input.GetAxisRaw("Vertical"));
            if (IsWalkable(transform.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f)))
            {
                animator.SetBool("isMoving", isMoving); //only set animator if actually moving
                companionAnim.SetBool("isMoving", isMoving);
                LastTileVisited = transform.position;
                movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }

        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            isMoving = false; animator.SetBool("isMoving", isMoving); companionAnim.SetBool("isMoving", isMoving);
        }
    }

    public void HandleUpdateFreeRoam() //handles player movement, collisions and battle encounters as well as companion position in free roam
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.R))
            OnUseHeal();
        if (Input.GetKey(KeyCode.Q))
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue($"You cannot use this ability outside of battle"));

        if (Vector3.Distance(transform.position, movePoint.position) <= .05f) //if not moving check for move inputs
        {
            CheckMoveInputs();
            OnMoveOver(); //check while not moving
           
        }
        else //while moving
        {
            //do not check move inputs until move over
            OnMoveOver(); //check while moving
        }
    }

    public void HandleUpdateBattle() //handles player movement and battle position of companion
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        
        //Allow player to move during battle without checking for battles, interactions and cutscenes
        if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
            CheckMoveInputs();
    }

    public void Interact() //allows player to interact on z press and if there is an npc in the facing dir;
    {
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPos = transform.position + facingDir;

        //interact if there is an npc in facing direction
        var collider = Physics2D.OverlapCircle(interactPos, 0.2f, npc);
        if (collider != null) //Is interactable object
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

    public void CheckForBoss()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.9f, boss); //should detect from a little more than a tile away
        if (collider != null)
        {
            Debug.Log("Colliding with boss");
            animator.SetBool("isMoving", false); companionAnim.SetBool("isMoving", false);
            StartBossFight();
        }
    }

    public void CheckForEnemies()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.9f, enemies ); //should detect from a little more than a tile away
        if (collider != null)
        {
            animator.SetBool("isMoving", false); companionAnim.SetBool("isMoving", false);
            OnEncounter?.Invoke(collider);
        }
    }

    private void CheckForCutscene()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, cutscene);
        if (collider != null)
        {
            animator.SetBool("isMoving", false); companionAnim.SetBool("isMoving", false);
            OnCutscene?.Invoke(collider);
        }
    }

    private void CheckForPortal()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, portal);
        if (collider != null )
        {
            if (!takingPortal)
            {
                takingPortal = true;
                Debug.Log("Entering Portal");
                Debug.Log(transform.position);
                animator.SetBool("isMoving", false); companionAnim.SetBool("isMoving", false);
                OnPortal(collider);
                //GetComponent<Portal>()?.OnPlayerTriggered(this);
            }
            else Debug.Log("Already taking portal");
        }
    }

    private void CheckForLevelLock()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.4f, levelLock);
        if (collider != null)
        {
            var levelLockObject = collider.GetComponent<LevelLock>();
            if (levelLockObject != null)
            {
                Debug.Log("Checking level lock");
                animator.SetBool("isMoving", false); companionAnim.SetBool("isMoving", false);
                levelLockObject.CheckLevelLock();
            }
        }
    }

    public void SetPositionAndSnapToTile(Vector2 position) //player transform getting updated elsewhere after this call on scene load why????
    {
        position.x = Mathf.Floor(position.x) + 0.5f;
        position.y = Mathf.Floor(position.y) + 0.8f;

        transform.position = position;
        movePoint.position = position;
    }
}
