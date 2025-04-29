using System.Collections.Generic;
using UnityEngine;

//This class is the implementation for character base 
public class Character : MonoBehaviour
{
    public string classType;
    [SerializeField] Weapon weapon;
    [SerializeField] HealthBar healthBar;
    [SerializeField] HealthBar xpBar;
    [SerializeField] SkillTree skillTree;

    //abilities
    [SerializeField] AbilityBase SwordsmanAbility;
    [SerializeField] AbilityBase MageAbility;
    [SerializeField] AbilityBase RogueAbility;
    [SerializeField] AbilityBase ArcherAbility;

    public int HP { get; set; }
    public int SkillPoints { get; set; }
    public bool targetable = true;
    
    public Weapon CurrentWeapon { get { return weapon; } }
    [SerializeField] List<WeaponBase> Weapons = new List<WeaponBase>();

    public WeaponBase debugWeapon;

    public void UpdateWeapon(WeaponBase newWeapon, string dialog)
    {
        if (newWeapon.Rarity.CompareTo("Divine") == 0) //always give new weapon if divine
        {
            GiveWeapon(newWeapon, dialog);
        }

        if (newWeapon.Rarity.CompareTo("Unparalled") == 0) //if new weapon is unparalled
        {
            if (weapon.WeaponBase.Rarity.CompareTo("Divine") != 0) //give if current weapon not better quality
            {
                GiveWeapon(newWeapon, dialog);
            }
            else { dialog = dialog.Substring(63); ShowLevelXPDialog(dialog); }
        }

        if (newWeapon.Rarity.CompareTo("Superior") == 0)
        {
            if (weapon.WeaponBase.Rarity.CompareTo("Divine") != 0)
            {
                if (weapon.WeaponBase.Rarity.CompareTo("Unparalled") != 0)
                {
                    GiveWeapon(newWeapon, dialog);
                }
                else { dialog = dialog.Substring(63); ShowLevelXPDialog(dialog); }
            }
            else { dialog = dialog.Substring(63); ShowLevelXPDialog(dialog); }
        }

        if (newWeapon.Rarity.CompareTo("Novel") == 0)
        {
            if (weapon.WeaponBase.Rarity.CompareTo("Divine") != 0)
            {
                if (weapon.WeaponBase.Rarity.CompareTo("Unparalled") != 0)
                {
                    if (weapon.WeaponBase.Rarity.CompareTo("Superior") != 0)
                    {
                        GiveWeapon(newWeapon, dialog);
                    }
                    else { dialog = dialog.Substring(63); ShowLevelXPDialog(dialog); }
                }
                else { dialog = dialog.Substring(63); ShowLevelXPDialog(dialog); }
            }
            else { dialog = dialog.Substring(63); ShowLevelXPDialog(dialog); }
        }

        if (newWeapon.Rarity.CompareTo("Irregular") == 0)
        {
            if (weapon.WeaponBase.Rarity.CompareTo("Divine") != 0)
            {
                if (weapon.WeaponBase.Rarity.CompareTo("Unparalled") != 0)
                {
                    if (weapon.WeaponBase.Rarity.CompareTo("Superior") != 0)
                    {
                        if (weapon.WeaponBase.Rarity.CompareTo("Novel") != 0)
                        {
                            GiveWeapon(newWeapon, dialog);
                        }
                        else { dialog = dialog.Substring(63); ShowLevelXPDialog(dialog); }
                    }
                    else { dialog = dialog.Substring(63); ShowLevelXPDialog(dialog); }
                }
                else { dialog = dialog.Substring(63); ShowLevelXPDialog(dialog); }
            }
            else { dialog = dialog.Substring(63); ShowLevelXPDialog(dialog); }
        }

    }

    void ShowLevelXPDialog(string dialog)
    {
        StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue(dialog));
    }

    public void GiveWeapon(WeaponBase newWeapon, string dialog)
    {
        weapon.UpdateWeaponBase(newWeapon);
        StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue(dialog));
    }

    public int Level { get; set; }

    public float XP { get; private set; }

    public float ATKmodifier { get; set; }
    public float DEFmodifier { get; set; }

    [SerializeField] CharacterBase cBase;
    public CharacterBase CharacterBase { get { return cBase; } }

    [SerializeField] List<AbilityBase> abilities = new List<AbilityBase>();
    public List<AbilityBase> Abilities { get { return abilities; } }

    //Level scaling formula = stat + (stat * x% * level)
    //current formulas are across all classes using the base to determine class' base stats
    //across all classes: aiming for roughly 8k health max, 8k max defense, 6k max attack around lvl 50
    //with stats reflecting the class, i.e swordsman has high health so its closer to 8k by max level
    public int Health
    {
        get { return Mathf.FloorToInt(cBase.Health + (cBase.Health * 0.56f * (Level - 1))); } //56% increase per level
    }
    public int Defense
    {
        get { return Mathf.FloorToInt(cBase.Defense + (cBase.Defense * 0.48f * (Level - 1) * (1 + (0.02f * skillTree.skillTree["Defense Bonus"]))) * DEFmodifier); } //48% increase per level ; 2% addition per defense bonus ; class ability modifier
    }
    public int Attack
    {
        get { return Mathf.FloorToInt(cBase.Attack + (cBase.Attack * 0.26f * (Level - 1) * (1 + (0.01f * skillTree.skillTree["Attack Bonus"]))) * ATKmodifier); } //26% increase per level ; 1% addition per attack bonus ; class ability modifier
    }

    public float HealAbilityPercentage { get { return 0.3f + (0.05f * skillTree.skillTree["Heal Buff"]); } } //30% + (5% * heal buff level) 

    //Start Method to set class type accordingly after character selection? 
    public void Start()
    {
        xpBar.SetHP(0);
        Level = 1;
        XP = 0;
        SkillPoints = 0;
        HP = Health;
        ATKmodifier = 1;
        DEFmodifier = 1;
        
        //GiveDebugStats();
        UpdateWeaponAndAbilities(classType);
        //if (debugWeapon != null) { weapon.UpdateWeaponBase(debugWeapon); }
    }

    void GiveDebugStats()
    {
        Level = 50;
        HP = Health;
        SkillPoints = 100;
    }

    public void GiveXP(float xpAmt)
    {
        XP += xpAmt;

        //set xp bar
        var nextLevelXP = LevelXP(Level + 1);
        if (XP > nextLevelXP)
            StartCoroutine(xpBar.SetHPSmooth(XP / LevelXP(Level + 3)));
        else if (XP == nextLevelXP)
            xpBar.SetHP(0);
        else
            StartCoroutine(xpBar.SetHPSmooth(XP / nextLevelXP));

        //determine if player should level up
        CheckLevel(XP);
    }

    void CheckLevel(float xp)
    {
        if (XP >= LevelXP(Level + 1) && XP < LevelXP(Level + 2))
        {
            var healthBeforeLevel = Health;
            Level += 1;
            var healthAfterLevel = Health;
            HP += healthAfterLevel - healthBeforeLevel;
        }
        else if (XP >= LevelXP(Level + 2))
        {
            var healthBeforeLevel = Health;
            Level += 2;
            var healthAfterLevel = Health;
            HP += healthAfterLevel - healthBeforeLevel; //gain health added on level up
        }
        SetPlayerHPBar((float)HP / Health);
    }
    float LevelXP(int level)
    {
        return Mathf.Pow(level, 3);
    }

    public void SetPlayerHPBar(float hpNormalized)
    {
        StartCoroutine(healthBar.SetHPSmooth(hpNormalized));
    }

    public void UpdateWeaponAndAbilities(string classType)
    {
        //set abilities and weapon based on classtype
        if (classType.CompareTo("Swordsman") == 0)
        {
            abilities[0] = SwordsmanAbility;
            weapon.UpdateWeaponBase(Weapons[0]);
        }
        else if (classType.CompareTo("Mage") == 0)
        {
            abilities[0] = MageAbility;
            weapon.UpdateWeaponBase(Weapons[1]);
        }
        else if (classType.CompareTo("Rogue") == 0)
        {
            abilities[0] = RogueAbility;
            weapon.UpdateWeaponBase(Weapons[2]);
        }
        else if (classType.CompareTo("Archer") == 0)
        {
            abilities[0] = ArcherAbility;
            weapon.UpdateWeaponBase(Weapons[3]);
        }
    }

    public DamageDetails TakeDamage(Enemy attacker)
    {
        bool isDead = false;
        float critical = 1f;
        if (UnityEngine.Random.value <= attacker.CritChance)
        {
            critical = 1.5f;
        }

        //damage taken formula : dmg = (abilityPower(%) * attackerATK) * (attackerATK / defenderDEF)
        int damageTaken = Mathf.FloorToInt((attacker.AbilityBase.Power * attacker.Attack) * ((float)attacker.Attack / Defense));
        damageTaken = Mathf.FloorToInt(damageTaken * critical);
        HP -= damageTaken;
        if (HP <= 0)
        {
            HP = 0;
            isDead = true;
        }

        var damageDetails = new DamageDetails() //character damage details
        {
            Fainted = isDead,
            Critical = critical,
            DamageTaken = damageTaken,
        };

        float HPnormalized;
        if (HP > 0)
        {
            HPnormalized = (float)HP / Health;
            StartCoroutine(healthBar.SetHPSmooth(HPnormalized));
        }
        else
            healthBar.SetHP(0);
        Debug.Log("Player took " + damageDetails.DamageTaken + " damage from enemy");
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
        int damageTaken = Mathf.FloorToInt(boss.Attack * ((float)boss.Attack / Defense) );
        damageTaken = Mathf.FloorToInt(damageTaken * critical);
        HP -= damageTaken;
        if (HP <= 0)
        {
            HP = 0;
            isDead = true;
        }

        var damageDetails = new DamageDetails() //character damage details
        {
            Fainted = isDead,
            Critical = critical,
            DamageTaken = damageTaken,
        };

        float HPnormalized;
        if (HP > 0)
        {
            HPnormalized = (float)HP / Health;
            StartCoroutine(healthBar.SetHPSmooth(HPnormalized));
        }
        else
            healthBar.SetHP(0);
        Debug.Log("Player took " + damageDetails.DamageTaken + " damage from boss");
        return damageDetails; //return damge info
    }
}
