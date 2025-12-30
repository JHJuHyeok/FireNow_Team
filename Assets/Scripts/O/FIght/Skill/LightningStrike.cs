using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [Header("Stats")]
    private float damage = 15f;
    private float range = 1f;

    [Header("Visual")]
    private float strikeDelay = 0.3f;
    private SpriteRenderer spriteRenderer;

    private bool hasStruck = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();


        if (GetComponent<Collider2D>() == null)
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = range;
            collider.enabled = false;
        }
    }

    public void Initialize(float damageRate, float strikeRange)
    {
        damage = 15f * damageRate;
        range = strikeRange;

        // 회전 초기화 (위에서 아래로)
        //transform.rotation = Quaternion.identity;

        CircleCollider2D collider = GetComponent<Collider2D>() as CircleCollider2D;
        if (collider != null)
        {
            collider.radius = range;
        }

        Invoke(nameof(Strike), strikeDelay);

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0.5f;
            spriteRenderer.color = color;
        }

        Destroy(gameObject, 1f);
    }

    private void Strike()
    {
        if (hasStruck) return;
        hasStruck = true;

        CircleCollider2D collider = GetComponent<Collider2D>() as CircleCollider2D;
        if (collider != null)
        {
            collider.enabled = true;
        }

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D col in hitEnemies)
        {
            if (col.CompareTag("Enemy"))
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        if (collider != null)
        {
            collider.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}