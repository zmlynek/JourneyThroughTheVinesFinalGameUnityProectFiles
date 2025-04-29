using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float lifetime;
    public float distance;
    public LayerMask whatStopsMovement;
    
    private PlayerController playerController;
    private BattleSystem battleSystem;

    private void Start()
    {
        battleSystem = FindFirstObjectByType<BattleSystem>();
        Invoke(nameof(DestroyProjectile), lifetime);
    }

    private void Update()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, distance, whatStopsMovement);
        if (hitInfo.collider != null)
        {
            playerController = FindAnyObjectByType<PlayerController>();
            if (hitInfo.collider.CompareTag("ENEMY"))
            {
                if (battleSystem != null)
                {
                    battleSystem.EnemyDamageDetails = hitInfo.collider.GetComponent<Enemy>().TakeDamage(playerController.PlayerChar);
                }
                else Debug.Log("Could not find BattleSystem");
            }
            else if (hitInfo.collider.CompareTag("BOSS"))
            {
                if (battleSystem != null)
                {
                    battleSystem.BossDamageDetails = hitInfo.collider.GetComponent<Boss>().TakeDamage(playerController.PlayerChar);
                }
                else Debug.Log("Could not find boss");
            }
            DestroyProjectile();
        }

        transform.Translate(speed * Time.deltaTime * Vector2.up);
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }

}
