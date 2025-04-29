using System.Collections;
using UnityEngine;

//This class  controls the companioons movement and states including dialogue and battle
public class CompanionController : MonoBehaviour
{
    [SerializeField]PlayerController playerController;
    [SerializeField] HealthBar healthBar;
    [SerializeField] SkillTree skillTree;
    public Transform movePoint;
    public Transform startingPos;
    public float moveSpeed = 5f;
    public LayerMask player;
    public LayerMask collision;
    public LayerMask enemies;

    public int MaxHealth { get { return Mathf.FloorToInt(200 + (200 * 0.5f * (Level-1))); } } //50% scaling per level
    public int Defense { get { return Mathf.FloorToInt(150 + (150 * 0.36f * (Level-1))); } } //36% scaling per level
    public int Attack { get { return Mathf.FloorToInt(100 + (100 * 0.22f * (Level-1))); } } //22% scaling per level
    public float CritChance { get; set; }
    public float Stregnth { get { return 0.3f + (0.1f * skillTree.skillTree["Companion Power"]); } }
    public int HP { get; set; }
    public int Level { get; set; }
    public float XP { get; set; }

    public void Start()
    {
        movePoint.parent = null;
        Level = 1;
        XP = 0f;
        HP = MaxHealth;
        CritChance = 0.01f;
        SetPositionAndSnapToTile(transform.position);

    }

    public void GiveXP(float xpAmt)
    {
        XP += xpAmt;
        //determine if player should level up
        CheckLevel(XP);
        
    }
    void CheckLevel(float xp)
    {
        if (XP >= LevelXP(Level + 1) && XP < LevelXP(Level + 2))
        {
            var healthBeforeLevel = MaxHealth;
            Level += 1;
            var healthAfterLevel = MaxHealth;
            HP += healthAfterLevel - healthBeforeLevel; //gain health added on level up
        }
        else if (XP >= LevelXP(Level + 2))
        {
            var healthBeforeLevel = MaxHealth;
            Level += 2;
            var healthAfterLevel = MaxHealth;
            HP += healthAfterLevel - healthBeforeLevel;
            SetCompanionHPBar((float)HP / MaxHealth);
        }
    }

    float LevelXP(int level)
    {
        return (4 * Mathf.Pow(level, 3)) / 5;
    }

    public void SetCompanionHPBar(float hpNormalized)
    {
        if (gameObject.activeSelf)
            StartCoroutine(healthBar.SetHPSmooth(hpNormalized));
    }

    public void ResetHealth()
    {
        HP = MaxHealth;
    }

    public void SetPositionAndSnapToTile(Vector2 position) //player transform getting updated elsewhere after this call on scene load why????
    {
        position.x = Mathf.Floor(position.x) + 0.5f;
        position.y = Mathf.Floor(position.y) - 0.2f;

        transform.position = position;
        movePoint.position = position;
    }

    private bool isWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.1f, player | collision | enemies) != null)
        {
            return false;
        }
        return true;
    }

    Vector3 FindAttackPos(Enemy enemy)
    {
        Transform enemyPos = enemy.transform;
        Vector3 targetTileR = new Vector3(enemyPos.position.x + 1, enemyPos.position.y);
        Vector3 targetTileL = new Vector3(enemyPos.position.x - 1, enemyPos.position.y);
        Vector3 targetTileU = new Vector3(enemyPos.position.x, enemyPos.position.y + 1);
        Vector3 targetTileD = new Vector3(enemyPos.position.x, enemyPos.position.y - 1);
        if (isWalkable(targetTileR))
            return targetTileR;
        else if (isWalkable(targetTileL))
            return targetTileL;
        else if (isWalkable(targetTileU))
            return targetTileU;
        else if (isWalkable(targetTileD))
            return targetTileD;
        else
            return transform.position;
    }

    Vector3 FindAttackPos(Boss boss)
    {
        Transform enemyPos = boss.transform;
        Vector3 targetTileR = new Vector3(enemyPos.position.x + 3, enemyPos.position.y);
        Vector3 targetTileL = new Vector3(enemyPos.position.x - 3, enemyPos.position.y);
        Vector3 targetTileU = new Vector3(enemyPos.position.x, enemyPos.position.y + 2);
        Vector3 targetTileD = new Vector3(enemyPos.position.x, enemyPos.position.y - 2);
        if (isWalkable(targetTileR)) 
            return targetTileR;
        else if (isWalkable(targetTileL)) 
            return targetTileL;
        else if (isWalkable(targetTileU))
            return targetTileU;
        else if (isWalkable(targetTileD))
            return targetTileD;
        else 
            return transform.position; 
    }
    public IEnumerator WalkToPos(Vector3 targetPos) //currently does not work for some reason
    {
        float originalX = transform.position.x;
        float originalY = transform.position.y;
        float xTiles = targetPos.x - originalX;
        float yTiles = targetPos.y - originalY;

        //movePoint + 1 tile along path to targetPos
        //for x-axis
        for (int i = 0; i < xTiles; i++)
        {
            var nextTile = new Vector3(movePoint.position.x + (originalX / Mathf.Abs(originalX)), originalY, transform.position.z);
            if (isWalkable(nextTile))
            {
                movePoint.position = nextTile;
                yield return new WaitForSeconds(0.2f);
            }
        }
        //for y-axis
        for (int i = 0; i < yTiles; i++)
        {
            var nextTile = new Vector3(originalX, movePoint.position.y + (originalY / Mathf.Abs(originalY)), transform.position.z);
            if (isWalkable(nextTile))
            {
                movePoint.position = new Vector3(originalX, movePoint.position.y + (originalY / Mathf.Abs(originalY)), transform.position.z);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    public DamageDetails TakeDamage(Enemy attacker)
    {
        Debug.Log("Companion taking damage from enemy");
        bool isDead = false;
        float critical = 1f;
        if (UnityEngine.Random.value <= attacker.CritChance)
        {
            critical = 1.5f;
        }

        //damage taken formula : dmg = (abilityPower(%) * attackerATK) * (attackerATK / defenderDEF)
        int damageTaken = Mathf.FloorToInt( (float)attacker.AbilityBase.Power * attacker.Attack * ( (float)attacker.Attack / Defense) );
        damageTaken = Mathf.FloorToInt(damageTaken * critical);
        HP -= damageTaken;
        if (HP <= 0)
        {
            HP = 0;
            isDead = true;
        }

         var damageDetails = new DamageDetails()
        {
            Fainted = isDead,
            Critical = critical,
            DamageTaken = damageTaken,
        };

        float HPnormalized;
        if (HP > 0)
        {
            HPnormalized = (float)HP / MaxHealth;
            StartCoroutine(healthBar.SetHPSmooth(HPnormalized));
        }
        else
            healthBar.SetHP(0);
        Debug.Log("Companion took " + damageDetails.DamageTaken + " from enemy");
        return damageDetails; //return damge info
    }

    public DamageDetails TakeDamage(Boss boss)
    {
        bool isDead = false;
        float critical = 1f;
        if (UnityEngine.Random.value <= boss.CritChance)
        {
            critical = 1.5f;
        }

        //damage taken formula : dmg = (abilityPower(%) * attackerATK) * (attackerATK / defenderDEF)
        int damageTaken = Mathf.FloorToInt(boss.Attack * ((float)boss.Attack / Defense));
        damageTaken = Mathf.FloorToInt(damageTaken * critical);
        HP -= damageTaken;
        if (HP <= 0)
        {
            HP = 0;
            isDead = true;
        }

        var damageDetails = new DamageDetails()
        {
            Fainted = isDead,
            Critical = critical,
            DamageTaken = damageTaken,
        };

        float HPnormalized;
        if (HP > 0)
        {
            HPnormalized = (float)HP / MaxHealth;
            StartCoroutine(healthBar.SetHPSmooth(HPnormalized));
        }
        else
            healthBar.SetHP(0);
        Debug.Log("Companion took " + damageDetails.DamageTaken + " from boss");
        return damageDetails; //return damge info
    }

    public IEnumerator BattleMode()
    {
        //Move to side of player instead of behind
        yield return new WaitForSeconds(1f);
        //Execute attacks every 3 seconds
        yield return new WaitForSeconds(3f);
    }

    public IEnumerator OnStartGame()
    {
        StartCoroutine(WalkToPos(startingPos.position));
        yield return new WaitForSeconds(2f);
    }

    public void HandleUpdateFreeRoam()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        //logic should be follow player if can move AND no dialogue
        if (Vector3.Distance(transform.position, movePoint.position) <= .05f) 
        {
            if (isWalkable(playerController.LastTileVisited)) //follow player
                movePoint.position = playerController.LastTileVisited;
        }
    }

    public void HandleUpdateBattle(Enemy enemy) //fix follow player on right ; maybe change to move to 1 tile range from enemy position
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        //logic should be follow player if can move AND no dialogue
        if (movePoint.position != FindAttackPos(enemy))
            movePoint.position = FindAttackPos(enemy);
    }

    public void HandleUpdateBattle(Boss boss) //fix follow player on right ; maybe change to move to 1 tile range from enemy position
    {        
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        //logic should be follow player if can move AND no dialogue
        if (movePoint.position != FindAttackPos(boss))
            movePoint.position = FindAttackPos(boss);
    }

    public void HandleUpdateCutscene()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime); //changed from Cutscene.StartCutscene()
    }
}
