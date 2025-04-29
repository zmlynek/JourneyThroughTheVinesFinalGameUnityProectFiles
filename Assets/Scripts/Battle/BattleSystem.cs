using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum BattleState { Battling, BossBattle, BattleOver, BossBattleOver, Busy }

//Methods to calculate battle ; used by GameController
public class BattleSystem : MonoBehaviour
{
    [SerializeField] UIHandler uiHandler;

    public DamageDetails CharDamageDetails;
    public DamageDetails CompanionDamageDetails;
    public DamageDetails BossDamageDetails { get; set; }
    public DamageDetails EnemyDamageDetails { get; set; }
    [SerializeField] Camera worldCam;

    //set companion + character from unity
    public CompanionController companion;
    public Character character;
    private PlayerController playerController;
    private SpriteRenderer psr;
    private Boss finalBoss;
    [SerializeField] SkillTree skillTree;

    [SerializeField] List<WeaponBase> classWeapons = new List<WeaponBase>(); //add code to update what bases are in this list based on class type
    [SerializeField] List<WeaponBase> SwordsmanClassWeapons = new List<WeaponBase>();
    [SerializeField] List<WeaponBase> MageClassWeapons = new List<WeaponBase>();
    [SerializeField] List<WeaponBase> RogueClassWeapons = new List<WeaponBase>();
    [SerializeField] List<WeaponBase> ArcherClassWeapons = new List<WeaponBase>();

    private Enemy Enemy { get; set; } //the current enemy involved in battle
    BattleState state;

    //set animators from unity
    public Animator playerAnimator;
    public Animator companionAnimator;
    public LayerMask enemies;
    public LayerMask boss;

    //locks for mutual exclusion
    private bool playerisAttacking = false;
    private bool companionisAttacking = false;
    private bool enemyisAttacking = false;
    private bool decisionMutex = false;
    private bool givingWeapon = false;

    public event Action<bool> OnBattleOver;
    public event Action<bool> OnBossBattleOver;

    private float timeBetweenShots;
    public float startTimeBetweenShots;
    public GameObject Projectile;
    public GameObject ArrowProjectile;
    public GameObject BossProjectile;
    public Transform shotPoint;
    public float rotationOffset; 
    public float bossRotationOffset;
    
    public void Start()
    {
        //character.SkillPoints = 0;
        playerController = character.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.OnUseHeal += UseHealAbilityFreeRoam;
            UpdateClassWeapons(playerController.PlayerChar.classType);
            var sr = playerController.GetComponent<SpriteRenderer>();
            if (sr != null) { psr = sr; } else { Debug.Log("Could not find sprite renderer"); }
        }
        else { Debug.Log("Could not find puzzle items"); }
    }

    public void StartBattle(Enemy enemy)
    {
        //Update current enemy and state
        state = BattleState.Battling;
        Enemy = enemy;

        //default damage details states where everyone is alive
        CharDamageDetails = new DamageDetails()
        {
            Fainted = false,
            Critical = 1f,
            DamageTaken = 0,
        };

        CompanionDamageDetails = new DamageDetails()
        {
            Fainted = false,
            Critical = 1f,
            DamageTaken = 0,
        };

        EnemyDamageDetails = new DamageDetails()
        {
            Fainted = false,
            Critical = 1f,
            DamageTaken = 0,
        };

        //reset battle bool variables
        playerisAttacking = false;
        companionisAttacking = false;
        enemyisAttacking = false;

        //Show battle start cutscene?? 

        //On first ever battle only, show battle explantion
        if (character.XP == 0)
        {
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This is a battle, the other cats are a little too confident and always let each other fight alone. That works out for us!"));
        }
    }

    public void StartBossBattle()
    {
        Debug.Log("Starting Boss Battle");
        finalBoss = FindFirstObjectByType<Boss>();
        if (finalBoss == null) { Debug.Log("Could not find boss"); }
         finalBoss.HealthBarObject.SetActive(true); 

        state = BattleState.BossBattle;
        CharDamageDetails = new DamageDetails()
        {
            Fainted = false,
            Critical = 1f,
            DamageTaken = 0,
        };

        CompanionDamageDetails = new DamageDetails()
        {
            Fainted = false,
            Critical = 1f,
            DamageTaken = 0,
        };

        BossDamageDetails = new DamageDetails()
        {
            Fainted = false,
            Critical = 1f,
            DamageTaken = 0,
        };

        //reset battle bool variables
        playerisAttacking = false;
        companionisAttacking = false;
        enemyisAttacking = false;
    }

    float GetCooldown(string classType)
    {
        float duration = character.Abilities[0].Cooldown;
        if (classType.CompareTo("Swordsman") == 0)
            duration -= ((skillTree.skillTree["Enrage Cooldown"] - 1) * 10f);
        else if (classType.CompareTo("Mage") == 0)
            duration -= ((skillTree.skillTree["Overdrive Cooldown"] - 1) * 10f);
        else if (classType.CompareTo("Rogue") == 0)
            duration -= ((skillTree.skillTree["Sneak Cooldown"] - 1) * 8f);
        else if (classType.CompareTo("Archer") == 0)
            duration -= ((skillTree.skillTree["Arrow Barrage Cooldown"] - 1) * 10f);

        return duration;
    }

    void UpdateClassWeapons(string classType)
    {
        if (classType.CompareTo("Swordsman") == 0)
            classWeapons = SwordsmanClassWeapons;
        else if (classType.CompareTo("Mage") == 0)
            classWeapons = MageClassWeapons;
        else if (classType.CompareTo("Rogue") == 0)
            classWeapons = RogueClassWeapons;
        else if (classType.CompareTo("Archer") == 0)
            classWeapons = ArcherClassWeapons;

        uiHandler.UpdateWeaponImage(character.CurrentWeapon.WeaponBase.WeaponSprite);
    }

    void UpdateUIStats()
    {
        if (character.HP != uiHandler.HP)
            uiHandler.UpdateHP(character.HP);

        if (character.Health != uiHandler.MaxHealth)
            uiHandler.UpdateMaxHP(character.Health);

        if (character.Attack != uiHandler.PlayerAttack)
            uiHandler.UpdateATK(character.Attack);

        if (character.Defense != uiHandler.PlayerDefense)
            uiHandler.UpdateDEF(character.Defense);

        if (companion.HP != uiHandler.CompanionHP)
            uiHandler.UpdateCompanionHP(companion.HP);
        
        if (companion.MaxHealth != uiHandler.CompanionMaxHealth)
            uiHandler.UpdateCompanionMaxHP(companion.MaxHealth);

        if (companion.Attack != uiHandler.CompanionAttack)
            uiHandler.UpdateCompanionATKValue(companion.Attack);

        if (companion.Defense != uiHandler.CompanionDefense)
            uiHandler.UpdateCompanionDEFValue(companion.Defense);

        if (character.SkillPoints != uiHandler.SkillPointsValue)
            uiHandler.UpdatePlayerSkillPoints(character.SkillPoints);

        if (character.Level != uiHandler.LevelValue)
            uiHandler.UpdatePlayerLevel(character.Level);

        if (companion.Level != uiHandler.CompanionLevel)
            uiHandler.UpdateCompanionLevel(companion.Level);

        uiHandler.UpdateWeaponRarity(character.CurrentWeapon.WeaponBase.Rarity);
        uiHandler.UpdateWeaponImage(character.CurrentWeapon.WeaponBase.WeaponSprite);

        character.SetPlayerHPBar((float)character.HP / character.Health);
        companion.SetCompanionHPBar((float)companion.HP / companion.MaxHealth);
    }

    void GiveXPToPlayerAndCompanion()
    {
        var xpToGiveToPlayer = (Enemy.Level * Enemy.EnemyBase.BaseXPYield) + (character.Level - 1 * 20);
        character.GiveXP(xpToGiveToPlayer);
        var xpToGiveToCompanion = (Enemy.Level * (Enemy.EnemyBase.BaseXPYield * 0.7f)) + (companion.Level - 1 * 20);
        companion.GiveXP(xpToGiveToCompanion);

        StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue($"You gained {xpToGiveToPlayer} xp, and your companion gained {xpToGiveToCompanion} xp!"));
        UpdateUIStats();
    }

    void CheckForWeaponUpgrade()
    {
        //give loot drops to player
        var lootDrop = Random.value;
        if (lootDrop <= 0.15f) //15% chance of dropping some weapon
        {
            givingWeapon = true;

            var rarity = Random.value;
            Debug.Log(rarity);
            if (rarity <= 0.6f) { GiveXPToPlayerAndCompanion(); } 
            else if (rarity > 0.6f && rarity <= 0.85f)   { ShowWeaponUpgradeAndXP(classWeapons[1]); } 
            else if (rarity > 0.85f && rarity <= 0.925f) { ShowWeaponUpgradeAndXP(classWeapons[2]); }
            else if (rarity > 0.925f && rarity <= 0.96f) { ShowWeaponUpgradeAndXP(classWeapons[3]); }
            else if (rarity > 0.96f && rarity <= 0.992f) { ShowWeaponUpgradeAndXP(classWeapons[4]); }
            else if (rarity > 0.992f && rarity <= 1f)    { ShowWeaponUpgradeAndXP(classWeapons[5]); }
        }
    }

    void ShowWeaponUpgradeAndXP(WeaponBase weaponUpgrade) //execute give xp in here to avoid dialog conflict
    {
        var xpToGiveToPlayer = (Enemy.Level * Enemy.EnemyBase.BaseXPYield) + (character.Level - 1 * 20);
        character.GiveXP(xpToGiveToPlayer);
        var xpToGiveToCompanion = (Enemy.Level * (Enemy.EnemyBase.BaseXPYield * 0.7f)) + (companion.Level - 1 * 20);
        companion.GiveXP(xpToGiveToCompanion);

        string newWeaponDialog = $"You have obtained a new weapon! You now have: {weaponUpgrade.name}. You gained {xpToGiveToPlayer} xp, and your companion gained {xpToGiveToCompanion} xp! ";
        playerController.PlayerChar.UpdateWeapon(weaponUpgrade, newWeaponDialog);
        UpdateUIStats();
    }

    void BossBattleOver(bool won)
    {
        if (won)
        {
            finalBoss.gameObject.SetActive(false);
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Companion: Congratulations! You beat the boss and now we can save our friend!"));
        }
        OnBossBattleOver(won);
    }

    void BattleOver(bool won) //return true if player won
    {
        if (won)
        {
            RemoveEnemyFromScene();
            CheckForWeaponUpgrade();

            //logic for loot drops, xp and skill points
            if (Enemy.EnemyBase.name.CompareTo("BasicOrange") == 0 && !givingWeapon) //is basicOrange
            {
                character.SkillPoints += 1;
                GiveXPToPlayerAndCompanion();
            }
            else if (Enemy.EnemyBase.name.CompareTo("BasicRagdoll") == 0 && !givingWeapon) //is basicRagdoll
            {
                character.SkillPoints += 2;
                GiveXPToPlayerAndCompanion();
            }

            givingWeapon = false;
        }

        OnBattleOver(won);
    }

    void RemoveEnemyFromScene()
    {
        Enemy.gameObject.SetActive(false);
        Enemy.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
    }

    public void HandleUpdate() //Handles each BattleState ; called by GameController during GameState.Battle
    {
        if (state == BattleState.Battling)
        {
            HandleBattle();
        }
        else if (state == BattleState.BossBattle)
        {
            HandleBossBattle();
        }
        else if (state == BattleState.BattleOver) //checking again here seems redundant, maybe remove state altogether and move to HandleBattle()
        {
            if (EnemyDamageDetails.Fainted) //enemy is dead
                { BattleOver(true); }
            else if (!EnemyDamageDetails.Fainted) //enemy not dead
                { BattleOver(false); }
        }
        else if (state == BattleState.BossBattleOver)
        {
            if (BossDamageDetails.Fainted)
                BossBattleOver(true);
            else if (!BossDamageDetails.Fainted)
                BossBattleOver(false);
        
        }
        
    }

    void HandleBossBattle()
    {
        if (!CharDamageDetails.Fainted)
        {
            if (!playerisAttacking)
            {
                CheckForBattleKeys();
                timeBetweenShots -= Time.deltaTime;
            }

            if (!enemyisAttacking && !BossDamageDetails.Fainted)
                StartCoroutine(BossDecideTargetAndAttack());

            if (!companionisAttacking && !CompanionDamageDetails.Fainted)
                StartCoroutine(AttackBoss());

            if (BossDamageDetails.Fainted)
                state = BattleState.BossBattleOver;
        }
        else { Debug.Log("Battle has ended"); state = BattleState.BossBattleOver; }
    }

    void HandleBattle()
    {
        //handle player win lose condition
        if (!CharDamageDetails.Fainted) //check for loss condition
        {
            //Check for player attack buttons
            if (!playerisAttacking)
            {
                CheckForBattleKeys();
                timeBetweenShots -= Time.deltaTime;
            }

            //Allow enemy to attack if alive
            if (!enemyisAttacking && !EnemyDamageDetails.Fainted)
                StartCoroutine(DecideTargetAndAttack());

            //Allow companion to attack if alive
            if (!companionisAttacking && !CompanionDamageDetails.Fainted)
                StartCoroutine(AttackEnemy());

            //enemy and player HP bars are adjusted accordingly within take damage methods
            //so hp bar update is local, each referencing their own

            //Check for player win condition
            if (EnemyDamageDetails.Fainted)
                state = BattleState.BattleOver;
        }
        else {state = BattleState.BattleOver; } //enemy win ; if player has no HP enemy will have HP and therfore enemyisDead will be false 
    }

    void CheckForBattleKeys() //determine how to battle based on player input
    {
        if (!CharDamageDetails.Fainted && !playerisAttacking) //if player is alive and not already attacking
        {
            if (Input.GetKeyDown(KeyCode.R))//Ability heal 
            {
                StartCoroutine(UseHealAbility());
            }
            else if (Input.GetKeyDown(KeyCode.Q)) //Class ability
            {
                Debug.Log("Using class ability");
                StartCoroutine(UseClassAbility());
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0)) //Main attack
            {
                var mousePosition = Input.mousePosition;
                var worldPosition = worldCam.ScreenToWorldPoint(mousePosition);
                worldPosition.z = 0; //now have 3d vector of world position to check with
                if (character.classType.CompareTo("Swordsman") == 0 || character.classType.CompareTo("Rogue") == 0)
                {
                    if (Physics2D.OverlapCircle(worldPosition, 0.2f, enemies | boss) != null) //if clicking on enemy
                    {
                        if (SceneManager.GetActiveScene().buildIndex != 11) { StartCoroutine(MainAttack()); }
                        else { StartCoroutine(MainBossAttack()); }
                        UpdateUIStats(); //to guarantee update of health ; may not be necessary
                    }
                    else
                    {
                        //miss logic should be show "miss" in place damage number once that is implemented
                        //implement miss logic here:
                        Debug.Log("Missed");
                    }
                }
                else if (character.classType.CompareTo("Mage") == 0) //Mage and Archer need different logic here
                {
                    Vector3 difference = worldPosition - playerController.transform.position; //distance between mouse click and player position
                    float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffset);

                    if (timeBetweenShots <= 0)
                    {
                        Instantiate(Projectile, shotPoint.position, transform.rotation); //change transform rotation to rotation around player
                        timeBetweenShots = startTimeBetweenShots;
                    }
                    else { timeBetweenShots -= Time.deltaTime; }
                }
                else if (character.classType.CompareTo("Archer") == 0)
                {
                    Vector3 difference = worldPosition - playerController.transform.position; //distance between mouse click and player position
                    float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffset);

                    if (timeBetweenShots <= 0)
                    {
                        Instantiate(ArrowProjectile, shotPoint.position, transform.rotation); //change transform rotation to rotation around player
                        timeBetweenShots = startTimeBetweenShots;
                    }
                    else { timeBetweenShots -= Time.deltaTime; }
                }
            }
        }
    }

    IEnumerator DecideTargetAndAttack() //enemy decides who to attack
    {
        if (!decisionMutex)
        {
            decisionMutex = true; //enter critical section

            if (!CharDamageDetails.Fainted && !CompanionDamageDetails.Fainted && character.targetable) //both player and companion are alive
            {
                var toAttack = UnityEngine.Random.value * 2; //range 0 to 2, if < 1 attack player, if > 1 attack companion
                if (toAttack < 1)
                {
                    yield return AttackPlayer();
                    yield return new WaitForSeconds(1.5f);
                }
                else if (toAttack >= 1)
                {
                    yield return AttackCompanion();
                    yield return new WaitForSeconds(1.5f);
                }
            }
            else if (!CharDamageDetails.Fainted) //only player is alive
            {
                yield return AttackPlayer();
                yield return new WaitForSeconds(1.5f);
            }
            else if (!character.targetable) //player untargetable ; no case for companion fighting alone
            {
                yield return AttackCompanion();
                yield return new WaitForSeconds(1.5f);
            }

            decisionMutex = false; //exit critcal section
        }
    }

    IEnumerator AttackPlayer() //Enemy attack player
    {
        if (character != null)
        {
            //enter critical section
            enemyisAttacking = true;

            yield return new WaitForSeconds(1.5f);
            //add logic to check for player location
            CharDamageDetails = character.TakeDamage(Enemy); //update character damagedetails information
            UpdateUIStats();

            //exit critical section
            enemyisAttacking = false;
        }
        else Debug.Log("BattleSystem AttackPlayer; Character is not found");
    }

    IEnumerator AttackCompanion() //Enemy attack companion
    {
        if (companion != null)
        {
            //enter critical section
            enemyisAttacking = true;

            yield return new WaitForSeconds(1.5f);

            CompanionDamageDetails = companion.TakeDamage(Enemy); //update companion damagedetails information
            UpdateUIStats();

            //exit critical section
            enemyisAttacking = false;
        }

    }

    IEnumerator BossDecideTargetAndAttack() //boss decides who to attack
    {
        if (!decisionMutex)
        {
            decisionMutex = true; //enter critical section
            if (!CharDamageDetails.Fainted && !CompanionDamageDetails.Fainted && character.targetable) //both player and companion are alive
            {
                var toAttack = UnityEngine.Random.value * 2; //range 0 to 2, if < 1 attack player, if > 1 attack companion
                if (toAttack < 1)
                {                   
                    Debug.Log("Boss decided player attack");
                    yield return BossDecidePlayerAttack();
                    yield return new WaitForSeconds(1.5f);
                }
                else if (toAttack >= 1)
                {
                    Debug.Log("Boss decided companion attack");
                    yield return BossDecideCompanionAttack();
                    yield return new WaitForSeconds(1.5f);
                }
            }
            else if (!CharDamageDetails.Fainted) //only player is alive
            {
                Debug.Log("Boss decided player attack");
                yield return BossDecidePlayerAttack();
                yield return new WaitForSeconds(1.5f);
            }
            else if (!character.targetable) //player untargetable ; no case for companion fighting alone
            {
                Debug.Log("Boss decided companion attack");
                yield return BossDecideCompanionAttack();
                yield return new WaitForSeconds(1.5f);
            }

            decisionMutex = false; //exit critcal section
        }

    }

    IEnumerator BossDecidePlayerAttack()
    {
        var rand = UnityEngine.Random.value;
        if (rand < 0.5f)
        {
            Debug.Log("Boss Decide Melee Player");
            yield return BossAttackPlayer();
        }
        else
        {
            //change targeting roation to player
            Vector3 difference = playerController.transform.position - shotPoint.position; //distance between mouse click and player position
            float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ + bossRotationOffset);

            Debug.Log("Boss Decide Ranged Attack Player");
            yield return BossRangedAttackPlayer();
        }
    }

    IEnumerator BossDecideCompanionAttack()
    {
        var rand = UnityEngine.Random.value;
        if (rand < 0.5f)
        {
            Debug.Log("Boss Decide Melee Companion");
            yield return BossAttackCompanion();
        }
        else
        {
            //change targeting roation to companion
            Vector3 difference = companion.transform.position - shotPoint.position; //distance between mouse click and player position
            float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ + bossRotationOffset);

            Debug.Log("Boss Decide Ranged Attack Companion");
            yield return BossRangedAttackCompanion();
        }
    }

    IEnumerator BossAttackPlayer() //Boss scratch player
    {
        if (character != null)
        {
            Debug.Log("Boss Melee Player");
            //enter critical section
            enemyisAttacking = true;

            yield return new WaitForSeconds(1.5f);
            //add logic to check for player location
            CharDamageDetails = character.TakeDamage(finalBoss); //update character damagedetails information
            UpdateUIStats();

            //exit critical section
            enemyisAttacking = false;
        }
        else Debug.Log("BattleSystem AttackPlayer; Character is not found");
    }

    IEnumerator BossRangedAttackPlayer()
    {
        Debug.Log("Boss Ranged Attack Player");
        Debug.Log(finalBoss.transform.position);
        Instantiate(BossProjectile, finalBoss.shotPoint.position, transform.rotation);
        yield return new WaitForSeconds(1.5f);
    }

    IEnumerator BossAttackCompanion() //Boss scratch companion
    {
        if (companion != null)
        {
            Debug.Log("Boss Melee Companion");
            //enter critical section
            enemyisAttacking = true;

            yield return new WaitForSeconds(1.5f);

            CompanionDamageDetails = companion.TakeDamage(finalBoss); //update companion damagedetails information
            UpdateUIStats();

            //exit critical section
            enemyisAttacking = false;
        }

    }

    IEnumerator BossRangedAttackCompanion()
    {
        Debug.Log("Boss Ranged Attack Companion");
        Debug.Log(finalBoss.transform.position);
        Instantiate(BossProjectile, finalBoss.shotPoint.position, transform.rotation);
        yield return new WaitForSeconds(1.5f);
    }

    IEnumerator AttackEnemy() //companion attack enemy
    {
        if (Enemy.currentHealth > 0 && !companionisAttacking)
        {
            if (Enemy != null)
            { 
                //enter critical section
                companionisAttacking = true;
            
                yield return new WaitForSeconds(3f);
                EnemyDamageDetails = Enemy.TakeDamage(companion); //update enemy damagedetails information
                UpdateUIStats();

                //exit critical section
                companionisAttacking = false;
            }
        }
    }

    IEnumerator AttackBoss() //companion attack boss
    {
        Debug.Log(finalBoss);
        Debug.Log(finalBoss.CurrentHealth);
        if (finalBoss.CurrentHealth > 0 && !companionisAttacking)
        {
            if (finalBoss != null)
            {
                //enter critical section
                companionisAttacking = true;

                yield return new WaitForSeconds(3f);
                BossDamageDetails = finalBoss.TakeDamage(companion); //update enemy damagedetails information
                UpdateUIStats();

                //exit critical section
                companionisAttacking = false;
            }
        }
    }

    IEnumerator MainAttack() //player main attack against enemy
    {
        //add logic to determine if attack is ranged or not ; different logic for ranged
        playerisAttacking = true; playerAnimator.SetBool("isAttacking", playerisAttacking); //update animator to play battle animation

        //enter critical Section ; make enemy take damage *depending on what ability* (to be added)
        if (!EnemyDamageDetails.Fainted)
        {
            EnemyDamageDetails = Enemy.TakeDamage(character); //update enemy damagedetails information //giving error and attacker passed is null
            UpdateUIStats();
        }            

        yield return new WaitForSeconds(0.5f);
        
        //exit critical section
        playerisAttacking = false; playerAnimator.SetBool("isAttacking", playerisAttacking); //update animator to end battle animation
    }

    IEnumerator MainBossAttack() //player main attack against enemy
    {
        //add logic to determine if attack is ranged or not ; different logic for ranged
        playerisAttacking = true; playerAnimator.SetBool("isAttacking", playerisAttacking); //update animator to play battle animation

        //enter critical Section ; make enemy take damage *depending on what ability* (to be added)
        if (!BossDamageDetails.Fainted)
        {
            BossDamageDetails = finalBoss.TakeDamage(character); //update enemy damagedetails information //giving error and attacker passed is null
            UpdateUIStats();
        }

        yield return new WaitForSeconds(0.5f);

        //exit critical section
        playerisAttacking = false; playerAnimator.SetBool("isAttacking", playerisAttacking); //update animator to end battle animation
    }

    IEnumerator UseClassAbility() //execute class ability based on what class the player is
    {
        //class ability decision + logic
        if (uiHandler.classAbilityReady)
        {
            //which class is using it
            if(character.classType.CompareTo("Swordsman") == 0)
            {
                //use enrage
                Debug.Log("Using ENRAGE");
                StartCoroutine(UseEnrageAbility());
            }
            else if (character.classType.CompareTo("Mage") == 0)
            {
                //use overdrive
                Debug.Log("Using OVERDRIVE");
                StartCoroutine(UseOverdriveAbility());
            }
            else if (character.classType.CompareTo("Rogue") == 0)
            {
                //use sneak
                Debug.Log("Using SNEAK");
                StartCoroutine(UseSneakAbility());
            }
            else if (character.classType.CompareTo("Archer") == 0)
            {
                //use arrow barrage
                Debug.Log("Using ARROW BARRAGE");
                StartCoroutine(UseArrowBarrageAbility());
            }
            UpdateUIStats();
        }

        uiHandler.SetClassAbilityCooldownMax(Mathf.FloorToInt(GetCooldown(character.classType)));
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator UseEnrageAbility()
    {
        //calculate bonuses 
        character.ATKmodifier = 1.15f + ((skillTree.skillTree["Enrage Power"] - 1) * 0.15f);
        character.DEFmodifier = 1.15f + ((skillTree.skillTree["Enrage Power"] - 1) * 0.15f);
        UpdateUIStats();

        //apply bonuses for set period of time
        yield return new WaitForSeconds(25f);
        UpdateUIStats();

        //remove bonuses after that
        character.ATKmodifier = 1;
        character.DEFmodifier = 1;
        UpdateUIStats();
    }

    IEnumerator UseOverdriveAbility()
    {
        //increase ATK and ATKspd for a set period of time
        character.ATKmodifier = 1.15f + ((skillTree.skillTree["Overdrive Power"] - 1) * 0.1f);
        startTimeBetweenShots -= 0.1f + (0.1f *  (skillTree.skillTree["Overdrive Power"]-1));
        if (startTimeBetweenShots < 0) { startTimeBetweenShots = 0; }
        UpdateUIStats();

        yield return new WaitForSeconds(20f);

        character.ATKmodifier = 1;
        startTimeBetweenShots = 0.5f;
        UpdateUIStats();
    }

    IEnumerator UseSneakAbility()
    {
        //set character invulnerability and opacity
        character.targetable = false; 
        Color sneakColor = psr.color; 
        sneakColor.a = 0.5f; 
        psr.color = sneakColor;

        //determine sneak duration and wait that long
        var sneakDuration = 12f + (1 * (skillTree.skillTree["Sneak Duration"]-1));
        yield return new WaitForSeconds(sneakDuration);

        //unset character invulnerability and opacity
        character.targetable = true;
        sneakColor.a = 1f; 
        psr.color = sneakColor;
    }

    IEnumerator UseArrowBarrageAbility()
    {
        //find world position
        var mousePosition = Input.mousePosition;
        var worldPosition = worldCam.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0; //now have 3d vector of world position to check with
        
        //find vector to shoot projectile along
        Vector3 difference = worldPosition - playerController.transform.position; //distance between mouse click and player position
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffset);

        //arrow barrage
        var arrows = 3 + (1 * (skillTree.skillTree["Arrow Barrage Extra Arrows"]-1));
        for (int i = 0; i < arrows; i++)
        {
            Instantiate(ArrowProjectile, shotPoint.position, transform.rotation); //change transform rotation to rotation around player
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForEndOfFrame();
    }

    IEnumerator UseHealAbility() //heal player *+ companion*
    {
        //determine player amount to heal and update player health within character class
        var playerAmtToHeal = Mathf.FloorToInt(character.Health * character.HealAbilityPercentage);
        if (playerAmtToHeal + character.HP >= character.Health)
            character.HP = character.Health;
        else
            character.HP += playerAmtToHeal;

        //determine companion amount to heal and update companion health within companion class
        var companionAmtToHeal = Mathf.FloorToInt(companion.MaxHealth * character.HealAbilityPercentage);
        if (companionAmtToHeal + companion.HP >= companion.MaxHealth)
            companion.HP = companion.MaxHealth;
        else
            companion.HP += companionAmtToHeal;

        //Update UI and health bars
        UpdateUIStats();
        character.SetPlayerHPBar((float)character.HP / character.Health);
        companion.SetCompanionHPBar((float)companion.HP / companion.MaxHealth);

        //reset ability cooldown to max
        uiHandler.SetHealAbilityCooldownMax(character.Abilities[1].Cooldown);
            
        //disallow spamming to get second heal off one cast
        yield return new WaitForSeconds(0.5f);
    }

    void UseHealAbilityFreeRoam()
    {
        StartCoroutine(UseHealAbility());
    }
}
