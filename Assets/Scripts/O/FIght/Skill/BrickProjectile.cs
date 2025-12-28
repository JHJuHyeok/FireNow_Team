using UnityEngine;

public class BrickProjectile : MonoBehaviour
{
    [Header("Stats")]
    private float damage = 10f;
    private float speed = 5f;
    private float lifetime = 3f;

    [Header("Physics")]
    private float gravity = 9.8f;
    private Vector2 velocity;
    private Rigidbody2D rb;

    private bool hasHit = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }

        // Collider가 없으면 추가
        if (GetComponent<Collider2D>() == null)
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.3f;
        }
    }

    public void Initialize(float damageRate, float projectileSpeed, Vector2 direction)
    {
        damage = 10f * damageRate;
        speed = projectileSpeed;

        rb.gravityScale = 0;

        // 포물선 궤적을 위한 초기 속도 설정
        velocity = direction.normalized * speed;
        velocity.y += 5f; 

        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (!hasHit)
        {
            // 중력 적용
            velocity.y -= gravity * Time.fixedDeltaTime;

            // 이동
            rb.velocity = velocity;

            // 회전 (날아가는 방향으로)
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        // 적과 충돌 체크
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                hasHit = true;
                Destroy(gameObject, 0.1f); // 약간의 딜레이 후 파괴 (애니메이션용)
            }
        }
    }
}