using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour 
{
    //will contain the skill tree as a dictionary, and methods to be used on buttons in the skill tree
    [SerializeField] UIHandler uiHandler;
    [SerializeField] Character character;
    public Dictionary<string, int> skillTree { get; private set; }
    [SerializeField] List<Sprite> LevelSprites = new List<Sprite>();

    //need reference to all level images
    [SerializeField] Image StaffMasteryLevelImage; //+20 dmg ; MAX LVL: 5
    [SerializeField] Image SwordMasteryLevelImage; //+15 dmg ; MAX LVL: 5
    [SerializeField] Image DaggerMasteryLevelImage; //+20 dmg ; MAX LVL: 5
    [SerializeField] Image BowMasteryLevelImage; //+15 dmg ; MAX LVL: 5

    [SerializeField] Image HealBuffLevelImage; //+5% healing ; MAX LVL: 6
    [SerializeField] Image CompanionPowerLevelImage; //+10% increase to strength ; MAX LVL: 10

    [SerializeField] Image AttackBonusLevelImage; //+1% ATK ; MAX LVL: 10
    [SerializeField] Image DefenseBonusLevelImage; //+2% DEF ; MAX LVL: 10

    [SerializeField] Image WeaponDropChanceLevelImage; //+1% drop chance ; MAX LVL: 10
    [SerializeField] Image WeaponRarityQualityLevelImage; //+1% better drop rate ; MAX LVL: 5
    [SerializeField] Image HeroOfTheForestLevelImage; //+25 ATK and +25 DEF ; MAX LVL: 5

    [SerializeField] Image OverdrivePowerLevelImage; //+10% for effects ; MAX LVL: 3
    [SerializeField] Image OverdriveCooldownLevelImage; //-10s cooldown ; MAX LVL: 3
    [SerializeField] Image EnragePowerLevelImage; //+15% for effects ; MAX LVL: 3
    [SerializeField] Image EnrageCooldownLevelImage; //-10s cooldown ; MAX LVL: 3
    [SerializeField] Image SneakDurationLevelImage; //+1s sneak time ; MAX LVL: 3
    [SerializeField] Image SneakCooldownLevelImage; //-8s cooldown ; MAX LVL: 3
    [SerializeField] Image ArrowBarrageArrowsLevelImage; //+1 arrow in arrow barrage ; MAX LVL: 3
    [SerializeField] Image ArrowBarrageCooldownLevelImage;//-10s cooldown ; MAX LVL: 3

    [SerializeField] GameObject StaffMasteryLockObject;
    [SerializeField] GameObject SwordMasteryLockObject;
    [SerializeField] GameObject DaggerMasteryLockObject;
    [SerializeField] GameObject BowMasteryLockObject;
    [SerializeField] GameObject CompanionPowerLockObject;
    [SerializeField] GameObject AttackBonusLockObject;
    [SerializeField] GameObject DefenseBonusLockObject;
    [SerializeField] GameObject WeaponDropChanceLockObject;
    [SerializeField] GameObject WeaponRarityQualityLockObject;
    [SerializeField] GameObject HeroOfTheForestLockObject;
    [SerializeField] GameObject OverdrivePowerLockObject;
    [SerializeField] GameObject OverdriveCooldownLockObject;
    [SerializeField] GameObject EnragePowerLockObject;
    [SerializeField] GameObject EnrageCooldownLockObject;
    [SerializeField] GameObject SneakDurationLockObject;
    [SerializeField] GameObject SneakCooldownLockObject;
    [SerializeField] GameObject ArrowBarrageArrowsLockObject;
    [SerializeField] GameObject ArrowBarrageCooldownLockObject;

    public event Action OnFailedUpgrade;
    public void Start()
    {
        //initialize everything to 1 and initialize dictionary
        skillTree = new Dictionary<string, int>();
        
        skillTree.Add("Staff Mastery", 1);
        skillTree.Add("Sword Mastery", 1);
        skillTree.Add("Dagger Mastery", 1);
        skillTree.Add("Bow Mastery", 1);

        skillTree.Add("Heal Buff", 1);
        skillTree.Add("Companion Power", 1);

        skillTree.Add("Attack Bonus", 1);
        skillTree.Add("Defense Bonus", 1);

        skillTree.Add("Weapon Drop Chance", 1);
        skillTree.Add("Weapon Rarity Quality", 1);
        skillTree.Add("Hero of the Forest", 1);

        skillTree.Add("Overdrive Power", 1);
        skillTree.Add("Overdrive Cooldown", 1);
        skillTree.Add("Enrage Power", 1);
        skillTree.Add("Enrage Cooldown", 1);
        skillTree.Add("Sneak Duration", 1); 
        skillTree.Add("Sneak Cooldown", 1);
        skillTree.Add("Arrow Barrage Extra Arrows", 1);
        skillTree.Add("Arrow Barrage Cooldown", 1);

        //skillTree.TryGetValue("Heal Buff", out var toPrint); //this code works to get value, so does skillTree["Skill Name"]

    }

    void UpdateLevelImage(Image levelToUpdate, string skillToUpdate)
    {
        levelToUpdate.sprite = LevelSprites[skillTree[skillToUpdate]-1];
        UpdateUISkillPoints();
    }

    void UpdateUISkillPoints()
    {
        uiHandler.UpdatePlayerSkillPoints(character.SkillPoints); uiHandler.UpdateSkillTreeSkillPoints(character.SkillPoints);
    }

    //button methods
    public void IncreaseHealBuffPoints()
    {
        //first skill to unlock only checks if there are enough skill points and not exceeding max level
        if (character.SkillPoints >= 1)
        {
            if (skillTree["Heal Buff"] < 6)
            {
                skillTree["Heal Buff"] += 1;
                character.SkillPoints -= 1;
                UpdateLevelImage(HealBuffLevelImage, "Heal Buff");
                if (skillTree["Heal Buff"] == 2)
                {
                    CompanionPowerLockObject.SetActive(false);
                    AttackBonusLockObject.SetActive(false);
                    DefenseBonusLockObject.SetActive(false);
                    OverdrivePowerLockObject.SetActive(false);
                    EnragePowerLockObject.SetActive(false);
                    SneakDurationLockObject.SetActive(false);
                    ArrowBarrageArrowsLockObject.SetActive(false);
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
        }
    }

    public void IncreaseCompanionPowerPoints()
    {
        if (skillTree["Heal Buff"] > 1) //if skill is unlocked
        {
            if (character.SkillPoints >= 1)
            { 
                if (skillTree["Companion Power"] < 10) //if has enough skill points and not at max level
                {
                    skillTree["Companion Power"] += 1;
                    character.SkillPoints -= 1;
                    UpdateLevelImage(CompanionPowerLevelImage, "Companion Power");
                    if (skillTree["Companion Power"] == 2)
                    {
                        StaffMasteryLockObject.SetActive(false);
                        SwordMasteryLockObject.SetActive(false);
                        DaggerMasteryLockObject.SetActive(false);
                        BowMasteryLockObject.SetActive(false);
                    }
                }
                else
                {
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }

    public void IncreaseAttackBonusPoints()
    {
        if (skillTree["Heal Buff"] > 1)
        {
            if (character.SkillPoints >= 1)
            {
                if (skillTree["Attack Bonus"] < 10)
                {
                    skillTree["Attack Bonus"] += 1;
                    character.SkillPoints -= 1;
                    UpdateLevelImage(AttackBonusLevelImage, "Attack Bonus");
                    if (skillTree["Attack Bonus"] == 2)
                    {
                        WeaponDropChanceLockObject.SetActive(false);
                        WeaponRarityQualityLockObject.SetActive(false);
                        HeroOfTheForestLockObject.SetActive(false);
                    }
                }
                else
                {
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }

    public void IncreaseDefenseBonusPoints()
    {
        if (skillTree["Heal Buff"] > 1)
        {
            if (character.SkillPoints >= 1)
            {
                if (skillTree["Defense Bonus"] < 10)
                {
                    skillTree["Defense Bonus"] += 1;
                    character.SkillPoints -= 1;
                    UpdateLevelImage(DefenseBonusLevelImage, "Defense Bonus");
                    if (skillTree["Defense Bonus"] == 2)
                    {
                        WeaponDropChanceLockObject.SetActive(false);
                        WeaponRarityQualityLockObject.SetActive(false);
                        HeroOfTheForestLockObject.SetActive(false);
                    }
                }
                else
                {
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }

    public void IncreaseWeaponDropChancePoints()
    {
        if (skillTree["Attack Bonus"] > 1 || skillTree["Defense Bonus"] > 1)
        {
            if (character.SkillPoints >= 1)
            {
                if (skillTree["Weapon Drop Chance"] < 10)
                {
                    skillTree["Weapon Drop Chance"] += 1;
                    character.SkillPoints -= 1;
                    UpdateLevelImage(WeaponDropChanceLevelImage, "Weapon Drop Chance");
                }
                else
                {
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }

    public void IncreaseWeaponRarityQualityPoints()
    {
        if (skillTree["Attack Bonus"] > 1 || skillTree["Defense Bonus"] > 1)
        {
            if (character.SkillPoints >= 1)
            { 
                if (skillTree["Weapon Rarity Quality"] < 5)
                {
                    skillTree["Weapon Rarity Quality"] += 1;
                    character.SkillPoints -= 1;
                    UpdateLevelImage(WeaponDropChanceLevelImage, "Weapon Rarity Quality");
                }
                else
                {
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }

    public void IncreaseHeroOfTheForestPoints()
    {
        if (skillTree["Attack Bonus"] > 1 || skillTree["Defense Bonus"] > 1)
        {
            if (character.SkillPoints >= 1)
            {
                if (skillTree["Hero of the Forest"] < 5)
                {
                    skillTree["Hero of the Forest"] += 1;
                    character.SkillPoints -= 1;
                    UpdateLevelImage(HeroOfTheForestLevelImage, "Hero of the Forest");
                }
                else
                {
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }

    public void IncreaseStaffMasteryPoints()
    {
        if (skillTree["Companion Power"] > 1)
        {
            if (character.SkillPoints >= 1) //if skill is unlocked and enough skill Points
            {
                if (skillTree["Staff Mastery"] < 5)
                {
                    if (character.classType.CompareTo("Mage") == 0)
                    {
                        skillTree["Staff Mastery"] += 1;
                        character.SkillPoints -= 1;
                        UpdateLevelImage(StaffMasteryLevelImage, "Staff Mastery");
                    }
                    else
                    {
                        OnFailedUpgrade();
                        StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("You have not selected the correct class to upgrade ability"));
                    }
                }
                else
                {
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }

    public void IncreaseSwordMasteryPoints()
    {
        if (skillTree["Companion Power"] > 1)
        {
            if (character.SkillPoints >= 1) //if skill is unlocked and enough skill Points
            {
                if (skillTree["Sword Mastery"] < 5)
                {
                    if (character.classType.CompareTo("Swordsman") == 0)
                    {
                        skillTree["Sword Mastery"] += 1;
                        character.SkillPoints -= 1;
                        UpdateLevelImage(SwordMasteryLevelImage, "Sword Mastery");
                    }
                    else
                    {
                        OnFailedUpgrade();
                        StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("You have not selected the correct class to upgrade ability"));
                    }
                }
                else
                {
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }
    public void IncreaseDaggerMasteryPoints()
    {
        if (skillTree["Companion Power"] > 1)
        {
            if (character.SkillPoints >= 1) //if skill is unlocked and enough skill Points
            {
                if (skillTree["Dagger Mastery"] < 5)
                {
                    if (character.classType.CompareTo("Rogue") == 0)
                    {
                        skillTree["Dagger Mastery"] += 1;
                        character.SkillPoints -= 1;
                        UpdateLevelImage(DaggerMasteryLevelImage, "Dagger Mastery");
                    }
                    else
                    {
                        OnFailedUpgrade();
                        StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("You have not selected the correct class to upgrade ability"));
                    }
                }
                else
                {
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }

    public void IncreaseBowMasteryPoints()
    {
        if (skillTree["Companion Power"] > 1)
        {
            if (character.SkillPoints >= 1) //if skill is unlocked and enough skill Points
            {
                if (skillTree["Bow Mastery"] < 5)
                {
                    if (character.classType.CompareTo("Archer") == 0)
                    {
                        skillTree["Bow Mastery"] += 1;
                        character.SkillPoints -= 1;
                        UpdateLevelImage(BowMasteryLevelImage, "Bow Mastery");
                    }
                    {
                        OnFailedUpgrade();
                        StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("You have not selected the correct class to upgrade ability"));
                    }
                }
                else
                {
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }

    public void IncreaseOverdrivePowerPoints()
    {
        if (skillTree["Heal Buff"] > 1)
        {
            if (character.SkillPoints >= 1)
            {
                if (skillTree["Overdrive Power"] < 3)
                {
                    if (character.classType.CompareTo("Mage") == 0)
                    {
                        skillTree["Overdrive Power"] += 1;
                        character.SkillPoints -= 1;
                        UpdateLevelImage(OverdrivePowerLevelImage, "Overdrive Power");
                        if (skillTree["Overdrive Power"] == 2) { OverdriveCooldownLockObject.SetActive(false); }
                    }
                    else
                    {
                        OnFailedUpgrade();
                        StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("You have not selected the correct class to upgrade ability"));
                    }
                }
                else
                {
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }

    public void IncreaseOverdriveCooldownPoints()
    {
        if (skillTree["Overdrive Power"] > 1)
        {
            if (character.SkillPoints >= 1)
            { 
                if (skillTree["Overdrive Cooldown"] < 3)
                {
                    if (character.classType.CompareTo("Mage") == 0)
                    {
                        skillTree["Overdrive Cooldown"] += 1;
                        character.SkillPoints -= 1;
                        UpdateLevelImage(OverdriveCooldownLevelImage, "Overdrive Cooldown");
                    }
                    else
                    {
                        OnFailedUpgrade();
                        StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("You have not selected the correct class to upgrade ability"));
                    }
                }
                else
                {
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }

    public void IncreaseEnragePowerPoints()
    {
        if (skillTree["Heal Buff"] > 1)
        {
            if (character.SkillPoints >= 1)
            {
                if (skillTree["Enrage Power"] < 3)
                {
                    if (character.classType.CompareTo("Swordsman") == 0)
                    {
                        skillTree["Enrage Power"] += 1;
                        character.SkillPoints -= 1;
                        UpdateLevelImage(EnragePowerLevelImage, "Enrage Power");
                        if (skillTree["Enrage Power"] == 2) { EnrageCooldownLockObject.SetActive(false); }
                    }
                    else
                    {
                        OnFailedUpgrade();
                        StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("You have not selected the correct class to upgrade ability"));
                    }
                }
                else
                {
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }

    public void IncreaseEnrageCooldownPoints()
    {
        if (skillTree["Enrage Power"] > 1)
        {
            if (character.SkillPoints >= 1)
            {
                if (skillTree["Enrage Cooldown"] < 3)
                {
                    if (character.classType.CompareTo("Swordsman") == 0)
                    {
                        skillTree["Enrage Cooldown"] += 1;
                        character.SkillPoints -= 1;
                        UpdateLevelImage(EnrageCooldownLevelImage, "Enrage Cooldown");
                    }
                    else
                    {
                        OnFailedUpgrade();
                        StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("You have not selected the correct class to upgrade ability"));
                    }
                }
                else
                {
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }

    public void IncreaseSneakDurationPoints()
    {
        if (skillTree["Heal Buff"] > 1)
        {
            if (character.SkillPoints >= 1)
            { 
                if (skillTree["Sneak Duration"] < 3)
                {
                    if (character.classType.CompareTo("Rogue") == 0)
                    {
                        skillTree["Sneak Duration"] += 1;
                        character.SkillPoints -= 1;
                        UpdateLevelImage(SneakDurationLevelImage, "Sneak Duration");
                        if (skillTree["Sneak Duration"] == 2) { SneakCooldownLockObject.SetActive(false); }
                    }
                    else
                    {
                        OnFailedUpgrade();
                        StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("You have not selected the correct class to upgrade ability"));
                    }
                }
                else
                {
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }

    public void IncreaseSneakCooldownPoints()
    {
        if (skillTree["Sneak Duration"] > 1)
        {
            if (character.SkillPoints >= 1)
            {
                if (skillTree["Sneak Cooldown"] < 3)
                {
                    if (character.classType.CompareTo("Rogue") == 0)
                    {
                        skillTree["Sneak Cooldown"] += 1;
                        character.SkillPoints -= 1;
                        UpdateLevelImage(SneakCooldownLevelImage, "Sneak Cooldown");
                    }
                    else
                    {
                        OnFailedUpgrade();
                        StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("You have not selected the correct class to upgrade ability"));
                    }
                }
                else
                {
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }

    public void IncreaseArrowBarrageArrowsPoints()
    {
        if (skillTree["Heal Buff"] > 1)
        {
            if (character.SkillPoints >= 1)
            {
                if (skillTree["Arrow Barrage Extra Arrows"] < 3)
                {
                    if (character.classType.CompareTo("Archer") == 0)
                    {
                        skillTree["Arrow Barrage Extra Arrows"] += 1;
                        character.SkillPoints -= 1;
                        UpdateLevelImage(ArrowBarrageArrowsLevelImage, "Arrow Barrage Extra Arrows");
                        if (skillTree["Arrow Barrage Extra Arrows"] == 2) { ArrowBarrageCooldownLockObject.SetActive(false); }
                    }
                    else
                    {
                        OnFailedUpgrade();
                        StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("You have not selected the correct class to upgrade ability"));
                    }
                }
                else
                {
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }

    public void IncreaseArrowBarrageCooldownPoints()
    {
        if (skillTree["Arrow Barrage Extra Arrows"] > 1)
        {
            if (character.SkillPoints >= 1)
            {
                if (skillTree["Arrow Barrage Cooldown"] < 3)
                {
                    if (character.classType.CompareTo("Archer") == 0)
                    {
                        skillTree["Arrow Barrage Cooldown"] += 1;
                        character.SkillPoints -= 1;
                        UpdateLevelImage(ArrowBarrageCooldownLevelImage, "Arrow Barrage Cooldown");
                    }
                    else
                    {
                        OnFailedUpgrade();
                        StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("You have not selected the correct class to upgrade ability"));
                    }
                }
                else 
                { 
                    OnFailedUpgrade();
                    StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability is at the maximum level"));
                }
            }
            else
            {
                OnFailedUpgrade();
                StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("Not enough skill points."));
            }
        }
        else
        {
            OnFailedUpgrade();
            StartCoroutine(DialogueManager.Instance.TypeSingleLineDialogue("This ability has not been unlocked yet."));
        }
    }

}
