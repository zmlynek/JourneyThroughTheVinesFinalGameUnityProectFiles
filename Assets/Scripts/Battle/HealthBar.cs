using System.Collections;
using UnityEngine;

//class that updates the scale of an object to display health in game
public class HealthBar: MonoBehaviour
{
    [SerializeField] GameObject health;

    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
    }

    public IEnumerator SetHPSmooth(float newHP)
    {
        float curHP = health.transform.localScale.x;
        float change = curHP - newHP;

        while (curHP - newHP > Mathf.Epsilon)
        {
            curHP -= (change * Time.deltaTime) * 3;
            health.transform.localScale = new Vector3(curHP, 1f);
            yield return null;
        }
        health.transform.localScale = new Vector3(newHP, 1f);
    }
}
