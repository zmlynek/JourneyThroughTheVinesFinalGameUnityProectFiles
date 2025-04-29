using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Ability/AbilityBase")]

//This is the base class for ability to create scriptable objects from the unity right click menu
public class AbilityBase : ScriptableObject
{
    [SerializeField] string type;
    [SerializeField] float power;
    [SerializeField] float duration;
    [SerializeField] int cooldown;
    [SerializeField] string abilityName;

    public string Type
    {
        get { return type; }
    }

    public string Name
    {
        get { return abilityName; }
    }

    public float Power
    {
        get { return power; }
    }

    public float Duration
    {
        get { return duration; }
    }
    public int Cooldown
    {
        get { return cooldown; }
    }


}
