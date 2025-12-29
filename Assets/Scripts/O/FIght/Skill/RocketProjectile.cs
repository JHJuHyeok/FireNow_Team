using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    [Header("Stats")]
    private float damage = 20f;
    private float speed = 8f;
    private float explosionRange = 2f;
    private float lifetime = 5f;

    [Header("Explosion")]
    public GameObject explosionEffectPrefab; // 폭발 이펙트 (선택사항)

    private Vector2 direction;
    private Rigidbody2D rb;
    private bool hasExploded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }

        if (GetComponent<Collider2D>() == null)
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.3f;
        }
    }

    public void Initialize(float damageRate, float projectileSpeed, float range, Vector2 dir)
    {
        damage = 20f * damageRate;
        speed = projectileSpeed * 6f;
        explosionRange = range;
        direction = dir.normalized;

        rb.velocity = direction * speed;

        // 회전 (날아가는 방향으로)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded) return;

        if (collision.CompareTag("Enemy"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // 폭발 범위 내 모든 적에게 데미지
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRange);

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

        // 폭발 이펙트 생성 (선택사항)
        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 1f);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // 폭발 범위 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}