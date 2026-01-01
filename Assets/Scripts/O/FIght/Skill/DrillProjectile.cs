using UnityEngine;

public class DrillProjectile : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite normalSprite;      // 일반 드릴
    public Sprite evolutionSprite;   // 메가 드릴 (진화)

    [Header("Stats")]
    private float damage = 10f;
    private float speed = 5f;
    private float lifetime = 6f;

    [Header("Bounce Settings")]
    private Camera mainCamera;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; //  추가
    private float damageCooldown = 0.2f;
    private float lastDamageTime = 0f;
    private string hitSoundName; // 추가


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

        //  SpriteRenderer 가져오기
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (GetComponent<Collider2D>() == null)
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.3f;
        }
    }
    // 히트 사운드 설정 메서드 추가
    public void SetHitSound(string soundName)
    {
        hitSoundName = soundName;
    }
    //  진화 여부에 따라 스프라이트 설정
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

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        CheckScreenBoundsAndReflect();
    }
    // 히트 사운드 재생 메서드 추가
    private void PlayHitSound()
    {
        if (string.IsNullOrEmpty(hitSoundName)) return;

        AudioClip clip = Resources.Load<AudioClip>($"SFX/Battle/Bulets/{hitSoundName}");
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position, 0.5f);
        }
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

            float angle = Mathf.Atan2(reflectedVelocity.y, reflectedVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
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
                PlayHitSound(); // 추가
                lastDamageTime = Time.time;
                return;
            }

            BossEnemy boss = collision.GetComponent<BossEnemy>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                PlayHitSound(); // 추가
                lastDamageTime = Time.time;
            }
        }
    }
}