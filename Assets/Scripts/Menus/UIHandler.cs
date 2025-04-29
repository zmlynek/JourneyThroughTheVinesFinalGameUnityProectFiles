using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    // this class will control the values shown on the gui
    // as well as the updates to the skill tree and items pages

    //GUI objects references (replace with references to skill tree and items pages)
    [SerializeField] GameObject gameUI;
    [SerializeField] GameObject menuUI;

    //GUI data to be updated
    //Text Fields
    [SerializeField] TextMeshProUGUI weaponRarity;
    [SerializeField] TextMeshProUGUI classAbilityText;
    [SerializeField] TextMeshProUGUI classAbilityCooldown;
    [SerializeField] TextMeshProUGUI healAbilityCooldown;

    //Number Fields
    [SerializeField] TextMeshProUGUI hpValue;
    [SerializeField] TextMeshProUGUI maxHPValue;
    [SerializeField] TextMeshProUGUI defValue;
    [SerializeField] TextMeshProUGUI atkValue;
    [SerializeField] TextMeshProUGUI companionHPValue;
    [SerializeField] TextMeshProUGUI companionMaxHPValue;
    [SerializeField] TextMeshProUGUI companionATKValue;
    [SerializeField] TextMeshProUGUI companionDEFValue;
    [SerializeField] TextMeshProUGUI levelValue;
    [SerializeField] TextMeshProUGUI companionLevelValue;
    [SerializeField] TextMeshProUGUI skillPointsValue;
    [SerializeField] TextMeshProUGUI skillTreeSkillPointsValue;

    //Health Bar Objects
    public HealthBar playerHealthBar;
    public HealthBar companionHealthBar;

    //Images
    [SerializeField] List<Sprite> classAbilityImages = new List<Sprite>();
    [SerializeField] Image classAbilityImage;
    [SerializeField] Image WeaponImage;

    //needed for cooldown update methods
    [SerializeField] GameObject classAbilityCooldownObject;
    [SerializeField] GameObject healAbilityCooldownObject;
    [SerializeField] GameObject classAbilityCooldownReady;
    [SerializeField] GameObject healAbilityCooldownReady;
    public int lastClassAbilityCooldown { get; private set; }
    public int lastHealAbilityCooldown { get; private set; }

    //mutex booleans
    private bool isUpdating = false;
    private bool hasReadClassCooldown = false;
    private bool hasReadHealCooldown = false;

    //signal for battle system
    public bool classAbilityReady = false;
    public bool healAbilityReady = false;

    public void UpdateHP(int newValue)
    {
        hpValue.text = newValue.ToString();
    }
    public int HP { get { return Int32.Parse(hpValue.text); } }

    public void UpdateMaxHP(int newValue)
    {
        maxHPValue.text = newValue.ToString();
    }
    public int MaxHealth { get { return Int32.Parse(maxHPValue.text); } }

    public void UpdateCompanionHP(int newValue)
    {
        companionHPValue.text = newValue.ToString();
    }
    public int CompanionHP { get { return Int32.Parse(companionHPValue.text); } }

    public void UpdateCompanionMaxHP(int newValue)
    {
        companionMaxHPValue.text = newValue.ToString();
    }
    public int CompanionMaxHealth { get { return Int32.Parse (companionMaxHPValue.text); } }

    public void UpdateDEF(int newValue)
    {
        defValue.text = newValue.ToString();
    }
    public int PlayerDefense { get { return Int32.Parse(defValue.text); } }

    public void UpdateATK(int newValue)
    {
        atkValue.text = newValue.ToString();
    }
    public int PlayerAttack { get { return Int32.Parse(atkValue.text); } }

    public void UpdateWeaponRarity(string newRarity)
    {
        weaponRarity.text = newRarity;
    }

    public void UpdatePlayerLevel(int newValue)
    {
        levelValue.text = newValue.ToString();
    }
    public int LevelValue { get { return Int32.Parse(levelValue.text); } }

    public void UpdateCompanionLevel(int newValue)
    {
        companionLevelValue.text = newValue.ToString();
    }
    public int CompanionLevel { get { return Int32.Parse(companionLevelValue.text); } }

    public void UpdateCompanionATKValue(int newValue)
    {
        companionATKValue.text = newValue.ToString();
    }
    public int CompanionAttack { get { return Int32.Parse(companionATKValue.text); } }
    
    public void UpdateCompanionDEFValue(int newValue)
    {
        companionDEFValue.text = newValue.ToString();
    }
    public int CompanionDefense { get { return Int32.Parse(companionDEFValue.text); } }

    public void UpdatePlayerSkillPoints(int newValue)
    {
        skillPointsValue.text = newValue.ToString();
        UpdateSkillTreeSkillPoints(newValue);
    }
    public int SkillPointsValue { get { return Int32.Parse(skillPointsValue.text); } }

    public void UpdateSkillTreeSkillPoints(int newValue)
    {
        skillTreeSkillPointsValue.text = newValue.ToString();
    }

    public void UpdateClassAbilityCooldown(int newValue)
    {
        classAbilityCooldown.text = newValue.ToString();
    }
    public void UpdateHealAbilityCooldown(int newValue)
    {
        healAbilityCooldown.text = newValue.ToString();
    }

    public void UpdateClassAbilityText(string newText)
    {
        classAbilityText.text = newText;
        if (newText.CompareTo("Arrow Barrage") == 0) { classAbilityText.fontSize = 30; }
    }

    public IEnumerator UpdateClassAbilityCooldownTimer() //is a timer for the cooldown
    {
        if (!hasReadClassCooldown)
        {
            if (Int32.TryParse(classAbilityCooldown.text, out var temp)) //get cooldown value
            {
                lastClassAbilityCooldown = temp;
                hasReadClassCooldown = true;
                if (lastClassAbilityCooldown > 0) //if is above 0
                {
                    lastClassAbilityCooldown -= 1; //decrement
                    UpdateClassAbilityCooldown(lastClassAbilityCooldown); //update with new value
                }
                else
                {
                    SetClassAbilityReady();
                }
            }
            else Debug.Log("Could not get cooldown timer");

            yield return new WaitForSeconds(0.5f); //wait 1 second to do again (since two coroutines each wait 0.5 seconds it makes 1)
            hasReadClassCooldown = false;
        }
    }

    public IEnumerator UpdateHealAbilityCooldownTimer() //is a timer for the cooldown
    {
        if (!hasReadHealCooldown) 
        {
            if (Int32.TryParse(healAbilityCooldown.text, out var temp)) //get cooldown value (currently not working)
            {
                lastHealAbilityCooldown = temp;
                hasReadHealCooldown = true;
                if (lastHealAbilityCooldown > 0) //if is above 0
                {
                    lastHealAbilityCooldown -= 1; //decrement
                    UpdateHealAbilityCooldown(lastHealAbilityCooldown); //update with new value
                }
                else
                {
                    SetHealAbilityReady();
                }
            }
            else Debug.Log("Could not get cooldown timer");

            yield return new WaitForSeconds(0.5f); //wait 1 second to do again
            hasReadHealCooldown = false;
        }        
    }

    void SetClassAbilityReady()
    {
        if (classAbilityCooldownObject.activeSelf)
        {
            classAbilityCooldownObject.SetActive(false);
            classAbilityCooldownReady.SetActive(true);
            classAbilityReady = true;
        }
    }
    public void SetClassAbilityCooldownMax(int cooldownMax) //set class ability cooldown to max
    {
        if (healAbilityCooldownReady.activeSelf) //if ability is ready
        {
            classAbilityCooldownObject.SetActive(true);
            classAbilityCooldownReady.SetActive(false);
            UpdateClassAbilityCooldown(cooldownMax);
            classAbilityReady = false;
        }
    }
    void SetHealAbilityReady()
    {
        if (healAbilityCooldownObject.activeSelf)
        {
            healAbilityCooldownObject.SetActive(false);
            healAbilityCooldownReady.SetActive(true);
            healAbilityReady = true;
        }
    }
    public void SetHealAbilityCooldownMax(int cooldownMax) //set heal ability cooldown to max
    {
        if (healAbilityCooldownReady.activeSelf) //if ability is ready
        {
            healAbilityCooldownObject.SetActive(true);
            healAbilityCooldownReady.SetActive(false);
            UpdateHealAbilityCooldown(cooldownMax);
            healAbilityReady = false;
        }
    }

    //not the best way of doing this, either replace with timer or one coroutine enveloping both class and ability cooldowns
    public IEnumerator HandleAbilityTimerUpdate() //Still works like this though
    {
        if (!isUpdating)
        {
            isUpdating = true;
            yield return StartCoroutine(UpdateClassAbilityCooldownTimer());
            yield return StartCoroutine(UpdateHealAbilityCooldownTimer());
            isUpdating = false;
        }
        yield return new WaitForEndOfFrame();
    }

    public void UpdateClassAbilityImage(string classType)
    {
        if(classType.CompareTo("Mage") == 0) { classAbilityImage.sprite = classAbilityImages[0]; UpdateClassAbilityText("Overdrive"); }
        else if (classType.CompareTo("Sowrdsman") == 0) { classAbilityImage.sprite = classAbilityImages[1]; UpdateClassAbilityText("Enrage"); }
        else if (classType.CompareTo("Rogue") == 0) { classAbilityImage.sprite = classAbilityImages[2]; UpdateClassAbilityText("Sneak"); }
        else if (classType.CompareTo("Archer") == 0) { classAbilityImage.sprite = classAbilityImages[3]; UpdateClassAbilityText("Arrow Barrage"); }
    }

    public void UpdateWeaponImage(Sprite imgSprite)
    {
        WeaponImage.sprite = imgSprite;
    }

    public void HideMapMenu()
    {
        menuUI.SetActive(false);
    }
}
