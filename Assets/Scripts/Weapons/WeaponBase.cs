using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/WeaponBase")]

//This is the base class for weapon to create scriptable objects from the unity right click menu
public class WeaponBase : ScriptableObject
{
    [SerializeField] int damage;
    [SerializeField] float critChance;
    [SerializeField] string rarity;
    [SerializeField] Sprite weaponSprite;

    public int Damage
    {
        get { return damage; }
    }

    public float CritChance
    {
        get { return critChance; }
    }

    public string Rarity
    {
        get { return rarity; }
    }

    public Sprite WeaponSprite
    {
        get { return weaponSprite; }
    }
}

