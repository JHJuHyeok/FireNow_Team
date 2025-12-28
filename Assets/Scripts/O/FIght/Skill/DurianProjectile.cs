using UnityEngine;

public class DurianProjectile : MonoBehaviour
{
    [Header("Stats")]
    private float damage = 12f;
    private float speed = 5f;
    private float lifetime = 6f;

    [Header("Bounce Settings")]
    private Camera mainCamera;

    [Header("Rotation")]
    private float rotationSpeed = 720f; // 초당 회전 각도

    private Rigidbody2D rb;

    // 데미지 쿨다운
    private float damageCooldown = 0.3f;
    private float lastDamageTime = 0f;

    private void Awake()
    {
        mainCamera = Camera.main;

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (GetComponent<Collider2D>() == null)
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.4f;
        }
    }

    public void Initialize(float damageRate, float projectileSpeed, Vector2 direction)
    {
        damage = 12f * damageRate;
        speed = projectileSpeed * 5f;

        rb.velocity = direction.normalized * speed;

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // 화면 경계 체크 및 반사
        CheckScreenBoundsAndReflect();

        // 계속 회전 (빙글빙글)
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void CheckScreenBoundsAndReflect()
    {
        if (mainCamera == null) return;

        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
        Vector2 currentVelocity = rb.velocity;
        bool needReflect = false;
        Vector2 normal = Vector2.zero;

        float boundary = 0.02f;

        // 좌측 벽
        if (viewportPosition.x <= boundary && currentVelocity.x < 0)
        {
            normal = Vector2.right;
            needReflect = true;
        }
        // 우측 벽
        else if (viewportPosition.x >= 1f - boundary && currentVelocity.x > 0)
        {
            normal = Vector2.left;
            needReflect = true;
        }

        // 하단 벽
        if (viewportPosition.y <= boundary && currentVelocity.y < 0)
        {
            normal = Vector2.up;
            needReflect = true;
        }
        // 상단 벽
        else if (viewportPosition.y >= 1f - boundary && currentVelocity.y > 0)
        {
            normal = Vector2.down;
            needReflect = true;
        }

        if (needReflect)
        {
            // 반사 공식
            Vector2 reflectedVelocity = Vector2.Reflect(currentVelocity, normal);
            rb.velocity = reflectedVelocity.normalized * speed;
            // 두리안은 회전 방향 유지 (변경 안 함)
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Time.time - lastDamageTime < damageCooldown)
            return;

        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                lastDamageTime = Time.time;
            }
        }
    }
}