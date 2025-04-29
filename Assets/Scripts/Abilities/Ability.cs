using Unity.VisualScripting;
using UnityEngine;

public class Ability 
{
    [SerializeField] AbilityBase aBase;

    public AbilityBase AbilityBase
    {
        get { return aBase; }
    }

    public string Type
    {
        get { return aBase.Type; }
    }

    public string Name
    {
        get { return aBase.Name; }
    }

    public float Power
    {
        get { return aBase.Power; }
    }

    public float Duration
    {
        get { return aBase.Duration; }
    }
    public int Cooldown
    {
        get { return aBase.Cooldown; }
    }
}
