using UnityEngine;

public class DurianProjectile : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite normalSprite;      // 일반 두리안
    public Sprite evolutionSprite;   // 진화 두리안

    [Header("Stats")]
    private float damage = 12f;
    private float speed = 5f;

    [Header("Bounce Settings")]
    private Camera mainCamera;

    [Header("Rotation")]
    private float rotationSpeed = 720f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
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

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (GetComponent<Collider2D>() == null)
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.4f;
        }
    }

    public void SetEvolution(bool isEvolution)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isEvolution ? evolutionSprite : normalSprite;
        }
    }

    public void Initialize(float finalDamage, float projectileSpeed, Vector2 direction)
    {
        damage = finalDamage;
        speed = projectileSpeed * 5f;
        rb.velocity = direction.normalized * speed;
    }

    private void Update()
    {
        CheckScreenBoundsAndReflect();
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

        if (viewportPosition.x <= boundary && currentVelocity.x < 0)
        {
            normal = Vector2.right;
            needReflect = true;
        }
        else if (viewportPosition.x >= 1f - boundary && currentVelocity.x > 0)
        {
            normal = Vector2.left;
            needReflect = true;
        }

        if (viewportPosition.y <= boundary && currentVelocity.y < 0)
        {
            normal = Vector2.up;
            needReflect = true;
        }
        else if (viewportPosition.y >= 1f - boundary && currentVelocity.y > 0)
        {
            normal = Vector2.down;
            needReflect = true;
        }

        if (needReflect)
        {
            Vector2 reflectedVelocity = Vector2.Reflect(currentVelocity, normal);
            rb.velocity = reflectedVelocity.normalized * speed;
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
                return;
            }

            BossEnemy boss = collision.GetComponent<BossEnemy>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                lastDamageTime = Time.time;
            }
        }
    }
}