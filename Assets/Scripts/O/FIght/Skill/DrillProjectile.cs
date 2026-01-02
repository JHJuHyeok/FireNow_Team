using UnityEngine;

public class DrillProjectile : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite evolutionSprite;

    [Header("Stats")]
    private float damage = 10f;
    private float speed = 5f;
    private float lifetime = 6f;

    [Header("Tracking Settings")]
    private bool isTracking = false;
    private Transform playerTransform;
    private float trackingSpeed = 8f;
    private float rotationSpeed = 200f;
    private Enemy currentTarget;
    private float retargetInterval = 0.5f;
    private float lastRetargetTime = 0f;

    [Header("Bounce Settings")]
    private Camera mainCamera;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float damageCooldown = 0.2f;
    private float lastDamageTime = 0f;
    private string hitSoundName;

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
            collider.radius = 0.3f;
        }
    }

    public void SetHitSound(string soundName)
    {
        hitSoundName = soundName;
    }

    public void SetEvolution(bool isEvolution)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isEvolution ? evolutionSprite : normalSprite;
        }
    }

    // 일반 초기화
    public void Initialize(float finalDamage, float projectileSpeed, Vector2 direction)
    {
        damage = finalDamage;
        speed = projectileSpeed * 5f;
        isTracking = false;

        rb.velocity = direction.normalized * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        Destroy(gameObject, lifetime);
    }

    // 추적 초기화 (진화 전용)
    public void InitializeTracking(float finalDamage, float trackSpeed, Transform player)
    {
        damage = finalDamage;
        trackingSpeed = trackSpeed;
        isTracking = true;
        playerTransform = player;

        // 수명 제거 (영구 존재)
        // Destroy 호출 안 함
    }

    private void Update()
    {
        if (isTracking)
        {
            TrackEnemy();
        }
        else
        {
            CheckScreenBoundsAndReflect();
        }
    }

    // 적 추적 로직
    private void TrackEnemy()
    {
        // 일정 간격으로 타겟 재설정
        if (Time.time - lastRetargetTime > retargetInterval || currentTarget == null)
        {
            currentTarget = FindNearestEnemy();
            lastRetargetTime = Time.time;
        }

        if (currentTarget != null)
        {
            // 적을 향해 이동
            Vector2 direction = (currentTarget.transform.position - transform.position).normalized;
            rb.velocity = direction * trackingSpeed;

            // 회전 (부드럽게)
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float currentAngle = transform.eulerAngles.z;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, newAngle);
        }
        else
        {
            // 타겟이 없으면 플레이어 주변을 맴돔
            if (playerTransform != null)
            {
                float distance = Vector2.Distance(transform.position, playerTransform.position);

                if (distance > 5f)
                {
                    // 플레이어에게 다가감
                    Vector2 direction = (playerTransform.position - transform.position).normalized;
                    rb.velocity = direction * trackingSpeed;
                }
                else
                {
                    // 플레이어 주변을 원형으로 맴돔
                    Vector2 tangent = Vector2.Perpendicular((transform.position - playerTransform.position).normalized);
                    rb.velocity = tangent * trackingSpeed * 0.5f;
                }
            }
        }
    }

    private Enemy FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Enemy nearest = null;
        float minDistance = float.MaxValue;
        float maxSearchRange = 15f; // 탐색 범위

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance && distance < maxSearchRange)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

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
                PlayHitSound();
                lastDamageTime = Time.time;

                // 추적 모드에서는 타겟이 죽으면 새 타겟 찾기
                if (isTracking && enemy == currentTarget)
                {
                    currentTarget = null;
                }
                return;
            }

            BossEnemy boss = collision.GetComponent<BossEnemy>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                PlayHitSound();
                lastDamageTime = Time.time;
            }
        }
    }
}