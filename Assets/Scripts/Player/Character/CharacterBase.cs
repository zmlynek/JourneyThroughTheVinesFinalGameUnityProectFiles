using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character/CharacterBase")]

//This is the base class for character to create scriptable objects from the unity right click menu
public class CharacterBase : ScriptableObject
{
    [SerializeField] int baseHealth;
    [SerializeField] int defense;
    [SerializeField] int attack;
    [SerializeField] int moveSpeed;

    [SerializeField] List<ClassAbilities> abilities;
    //[SerializeField] PlayerController playerController;

    public int Health
    {
        get { return baseHealth; }
    }
    public int Defense
    {
        get { return defense; }
    }
    public int Attack
    {
        get { return attack; }
    }
}

public class ClassAbilities
{
    [SerializeField] AbilityBase abilityBase;
    public AbilityBase AbilityBase
    {
        get { return abilityBase; }
    }

}
