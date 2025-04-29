using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

//This class is the implementation for enemy base ; contains methods for enemy movement and battle 
public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyBase eBase;
    [SerializeField] AbilityBase aBase;
    [SerializeField] int level;
    [SerializeField] Transform movePoint;
    public GameObject healthBarObject;
    public HealthBar healthBar;
    private bool isProwling = false;
    public LayerMask whatStopsMovement;
    public LayerMask companionLayer;

    public EnemyBase EnemyBase
    {
        get { return eBase; }
    }

    public AbilityBase AbilityBase 
    {
        get { return aBase; }
    }

    [SerializeField] List<Ability> attacks;

    public List<Ability> Attacks { get { return attacks; } }

    public int Level
    {
        get { return level; }
    }
    
    public float CritChance
    {
        get { return eBase.CritChance; }
    }

    public int currentHealth { get; set; }
    public int Health
    {
        get { return Mathf.FloorToInt(eBase.Health + (eBase.Health * 0.5f * (level - 1) )); } //50% increase per level
    }
    public int Defense
    {
        get { return Mathf.FloorToInt(eBase.Defense + (eBase.Defense * 0.42f * (level-1) )); } //42% increase per level
    }
    public int Attack
    {
        get { return Mathf.FloorToInt(eBase.Attack + (eBase.Attack * 0.25f * (level - 1) )); } //25% increase per level
    }

    public Vector3 FindProwlLocation() //find new location within 1 tiles
    {
        int newLocation = UnityEngine.Random.Range(1,2);
        int direction = UnityEngine.Random.Range(1,5);

        if (direction == 1)
            return new Vector3(transform.position.x, transform.position.y + newLocation, transform.position.z);
        else if (direction == 2)
            return new Vector3(transform.position.x, transform.position.y - newLocation, transform.position.z);
        else if (direction == 3)
            return new Vector3(transform.position.x + newLocation, transform.position.y, transform.position.z);
        else
            return new Vector3(transform.position.x - newLocation, transform.position.y, transform.position.z);

    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.1f, whatStopsMovement) != null)
        {
            return false;
        }
        return true;
    }

    IEnumerator Prowl() //Roam around a 3 tile radius, *checking for player*
    {
        isProwling = true;
        var startingPoint = transform.position; //Cache start position then move to new postion
        var newLocation = FindProwlLocation();
        if (IsWalkable(newLocation))
        {
            movePoint.position = newLocation;
            yield return new WaitForSeconds(3f); //Wait at new location before returning
            movePoint.position = startingPoint;
            yield return new WaitForSeconds(3f);
        }
        isProwling = false;
    }
   
    //TakeDamage overload
    public DamageDetails TakeDamage(CompanionController companion)
    {
        bool isDead = false;
        float HPnormalized;
        float critical = 1f;
        if (UnityEngine.Random.value <= companion.CritChance)
        {
            critical = 1.5f;
        }

        //enemy damage taken from companion formula : dmg = companionATK * companionSTR * (companionATK / enemyDEF)
        int damageTaken = Mathf.FloorToInt(companion.Attack * companion.Stregnth * ((float)companion.Attack / Defense) );
        damageTaken = Mathf.FloorToInt(damageTaken * critical);
        currentHealth -= damageTaken;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
        }

        var damageDetailed = new DamageDetails()
        {
            Fainted = isDead,
            Critical = critical,
            DamageTaken = damageTaken,
        };

        if (currentHealth > 0)
        {
            HPnormalized = (float)currentHealth / Health;
            StartCoroutine(healthBar.SetHPSmooth(HPnormalized));
        }
        else
            healthBar.SetHP(0);
        Debug.Log("Enemy took : " + damageDetailed.DamageTaken + " damage from companion");
        return damageDetailed;
    }

    int DeterminePlayerClass(Character character)
    {
        var playerClass = character.classType;
        int damageTaken = 0;
        if (playerClass.CompareTo("Swordsman") == 0)
            damageTaken = Mathf.FloorToInt(character.CurrentWeapon.SwordsmanWeaponDamage * (character.Attack / Defense));
        else if (playerClass.CompareTo("Mage") == 0)
            damageTaken = Mathf.FloorToInt(character.CurrentWeapon.MageWeaponDamage * (character.Attack / Defense));
        else if (playerClass.CompareTo("Rogue") == 0)
            damageTaken = Mathf.FloorToInt(character.CurrentWeapon.RogueWeaponDamage * (character.Attack / Defense));
        else if (playerClass.CompareTo("Archer") == 0)
            damageTaken = Mathf.FloorToInt(character.CurrentWeapon.ArcherWeaponDamage * (character.Attack / Defense));
        return damageTaken;
    }
    
    public DamageDetails TakeDamage(Character attacker) 
    {
        //add logic for if it is an ability instead of an attack?
        bool isDead = false;
        float HPnormalized;
        float critical = 1f;

        if (attacker.CurrentWeapon.WeaponBase.CritChance > 0)
            if (UnityEngine.Random.value <= attacker.CurrentWeapon.WeaponBase.CritChance)
            {
                critical = 1.5f;
            }

        //enemy damage taken from player formula : dmg = weaponDMG * (classATK / enemyDEF)
        int damageTaken = DeterminePlayerClass(attacker);
        damageTaken = Mathf.CeilToInt(damageTaken * critical); 
        currentHealth -= damageTaken;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
        }

        var damageDetailed = new DamageDetails()
        {
            Fainted = isDead,
            Critical = critical,
            DamageTaken = damageTaken,
        };

        if (currentHealth > 0)
        {
            HPnormalized = (float)currentHealth / Health;
            StartCoroutine(healthBar.SetHPSmooth(HPnormalized));
        }
        else
            healthBar.SetHP(0);
        Debug.Log("Enemy took : " + damageDetailed.DamageTaken + " damage from player");
        return damageDetailed; //return true if enemy (this) is dead
    }

    public void Start()
    {
        movePoint.parent = null;
        currentHealth = Health;
    }

    public void HandleUpdate()
    {
        healthBarObject.SetActive(false);
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, eBase.MoveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, movePoint.position) <= .05f && !isProwling) //&& !isProwling 
        {
            if(this.gameObject.activeSelf)
                StartCoroutine(Prowl());
        }

    }

    public void HandleUpdateBattle()
    {
        //Show health bar during battle
        if (!healthBarObject.activeSelf)
            healthBarObject.SetActive(true);
        
    }
    
}
