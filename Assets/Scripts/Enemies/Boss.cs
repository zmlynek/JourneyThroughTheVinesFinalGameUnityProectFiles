using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public GameObject HealthBarObject;
    public Transform shotPoint;
    [SerializeField] HealthBar healthBar;

    public float CritChance { get; set; }

    public int CurrentHealth { get; set; }
    public int Health { get; set; }
    public int Defense { get; set; }
    public int Attack { get; set; }
    [SerializeField] List<AbilityBase> abilities = new List<AbilityBase>();
    public List<AbilityBase> Abilities { get { return abilities; } }

    private void Start()
    {
        Debug.Log("initializing boss data");
        CritChance = 0.2f;
        CurrentHealth = Health = 10000;
        Defense = 1000;
        Attack = 750;
    }

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
        int damageTaken = Mathf.FloorToInt(companion.Attack * companion.Stregnth * ((float)companion.Attack / Defense));
        damageTaken = Mathf.FloorToInt(damageTaken * critical);
        CurrentHealth -= damageTaken;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            isDead = true;
        }

        var damageDetailed = new DamageDetails()
        {
            Fainted = isDead,
            Critical = critical,
            DamageTaken = damageTaken,
        };

        if (CurrentHealth > 0)
        {
            HPnormalized = (float)CurrentHealth / Health;
            StartCoroutine(healthBar.SetHPSmooth(HPnormalized));
        }
        else
            healthBar.SetHP(0);
        Debug.Log("Boss took " + damageDetailed.DamageTaken + " damage from companion");
        return damageDetailed;
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
        damageTaken = Mathf.FloorToInt(damageTaken * critical);
        CurrentHealth -= damageTaken;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            isDead = true;
        }

        var damageDetailed = new DamageDetails()
        {
            Fainted = isDead,
            Critical = critical,
            DamageTaken = damageTaken,
        };

        if (CurrentHealth > 0)
        {
            HPnormalized = (float)CurrentHealth / Health;
            StartCoroutine(healthBar.SetHPSmooth(HPnormalized));
        }
        else
            healthBar.SetHP(0);
        Debug.Log("Boss took " + damageDetailed.DamageTaken + " damage from player");
        return damageDetailed; //return true if enemy (this) is dead
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
}
