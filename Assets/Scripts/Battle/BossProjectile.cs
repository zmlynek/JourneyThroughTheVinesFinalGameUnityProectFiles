using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed;
    public float lifetime;
    public float distance;
    public LayerMask whatStopsMovement;

    private PlayerController playerController;
    private CompanionController companion;
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
            if (hitInfo.collider.CompareTag("PLAYER"))
            {
                playerController = FindAnyObjectByType<PlayerController>();
                if (battleSystem != null)
                {
                    var boss = hitInfo.collider.GetComponent<Boss>();
                    battleSystem.CharDamageDetails = playerController.PlayerChar.TakeDamage(boss);
                }
                else Debug.Log("Could not find BattleSystem");
            }
            else if (hitInfo.collider.CompareTag("COMPANION"))
            {
                companion = FindAnyObjectByType<CompanionController>();
                if (battleSystem != null)
                {
                    var boss = hitInfo.collider.GetComponent<Boss>();
                    battleSystem.CompanionDamageDetails = companion.TakeDamage(boss);
                }
                else Debug.Log("Could not find BattleSystem");
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
