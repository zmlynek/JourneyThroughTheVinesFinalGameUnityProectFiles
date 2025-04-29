using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy/EnemyBase")]

//This is the base class for enemy to create scriptable objects from the unity right click menu
public class EnemyBase : ScriptableObject
{
    [SerializeField] int baseHealth;
    [SerializeField] int defense;
    [SerializeField] int attack;
    [SerializeField] int moveSpeed;
    [SerializeField] float critChance;
    [SerializeField] float baseXpYield;

    public LayerMask player;
    public LayerMask companion;

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

    public int MoveSpeed
    {
        get { return moveSpeed; }
    }
    public float CritChance
    {
        get { return critChance; }
    }
    public float BaseXPYield
    {
        get { return baseXpYield; }
    }
}
