using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour, Interactable
{
    private Character character;
    private string classType;
    private SpriteRenderer itemImage;
    private UIHandler uiHandler;
    private int currentWeaponImage = 0;

    private WeaponBase weaponToGive;
    [SerializeField] List<WeaponBase> weaponsByClass = new List<WeaponBase>();
    [SerializeField] List<Sprite> weaponImages = new List<Sprite>(); 

    void Awake()
    {
        itemImage = GetComponent<SpriteRenderer>();
        character = FindFirstObjectByType<Character>();
        uiHandler = FindFirstObjectByType<UIHandler>();
        if (character == null) { Debug.Log("Could not find character"); }
        else {
            Debug.Log(character.classType);
            classType = character.classType;
            if (classType.CompareTo("Swordsman") == 0)
            {
                itemImage.sprite = weaponImages[0];
                weaponToGive = weaponsByClass[0];
                currentWeaponImage = 0;
            }
            else if (classType.CompareTo("Mage") == 0)
            {
                itemImage.sprite = weaponImages[1];
                weaponToGive = weaponsByClass[1];
                currentWeaponImage = 1;
            }
            else if (classType.CompareTo("Swordsman") == 0)
            {
                itemImage.sprite = weaponImages[2];
                weaponToGive = weaponsByClass[2];
                currentWeaponImage = 2;
            }
            else if (classType.CompareTo("Swordsman") == 0)
            {
                itemImage.sprite = weaponImages[3];
                weaponToGive = weaponsByClass[3];
                currentWeaponImage = 3;
            }
            else Debug.Log("Invalid ClassType");
        }
    }

    public void Interact()
    {
        character.GiveWeapon(weaponToGive, $"You have found a new weapon! You now have: {weaponToGive.name}.");
        uiHandler.UpdateWeaponImage(weaponImages[currentWeaponImage]);
        uiHandler.UpdateWeaponRarity(weaponToGive.Rarity);
        gameObject.SetActive(false);
    }

}
