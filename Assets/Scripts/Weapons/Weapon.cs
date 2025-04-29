using Unity.VisualScripting;
using UnityEngine;

//This class is what is referenced to do calculations for the correct weapon ??
public class Weapon : MonoBehaviour
{
    [SerializeField] WeaponBase wBase;
    [SerializeField] SkillTree skillTree;

    public WeaponBase WeaponBase
    {
        get { return wBase; }
    }

    public void UpdateWeaponBase(WeaponBase newBase)
    {
        wBase = newBase;
    }

    public int SwordsmanWeaponDamage
    {
        get { return wBase.Damage + (15 * skillTree.skillTree["Sword Mastery"]-1); }
    }
    public int MageWeaponDamage
    {
        get { return wBase.Damage + (20 * skillTree.skillTree["Staff Mastery"]-1); }
    }
    public int ArcherWeaponDamage
    {
        get { return wBase.Damage + (15 * skillTree.skillTree["Bow Mastery"]-1); }
    }
    public int RogueWeaponDamage
    {
        get { return wBase.Damage + (20 * skillTree.skillTree["Dagger Mastery"]-1); }
    }

}
