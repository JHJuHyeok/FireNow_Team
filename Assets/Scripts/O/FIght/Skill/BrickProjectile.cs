using UnityEngine;

public class BrickProjectile : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite evolutionSprite;

    [Header("Arc Settings")]
    [SerializeField] private float arcHeight = 3f; // 포물선 높이
    [SerializeField] private float arcDuration = 1f; // 날아가는 시간

    private float damage;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float elapsedTime = 0f;

    private SpriteRenderer spriteRenderer;
    private string hitSoundName;
    private bool isFlying = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Rigidbody2D는 포물선 운동에서 사용 안 함
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
        }
    }

    public void SetEvolution(bool isEvolution)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isEvolution ? evolutionSprite : normalSprite;
        }
    }

    public void Initialize(float finalDamage, float spd, Vector2 dir)
    {
        damage = finalDamage;
        startPosition = transform.position;

        // 목표 지점 계산 (현재 위치에서 dir 방향으로 일정 거리)
        float distance = spd * arcDuration;
        targetPosition = startPosition + dir * distance;

        isFlying = true;
        elapsedTime = 0f;
    }

    private void Update()
    {
        if (!isFlying) return;

        elapsedTime += Time.deltaTime;
        float t = elapsedTime / arcDuration;

        if (t >= 1f)
        {
            // 목표 지점 도달
            transform.position = targetPosition;
            Destroy(gameObject);
            return;
        }

        // 포물선 궤적 계산
        Vector2 currentPos = Vector2.Lerp(startPosition, targetPosition, t);

        // 위로 올라갔다가 내려오는 곡선 (sin 함수 사용)
        float heightOffset = Mathf.Sin(t * Mathf.PI) * arcHeight;
        currentPos.y += heightOffset;

        transform.position = currentPos;

        // 회전 (진행 방향을 향하도록)
        Vector2 velocity = (currentPos - (Vector2)transform.position).normalized;
        if (velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void SetHitSound(string soundName)
    {
        hitSoundName = soundName;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                PlayHitSound();
                Destroy(gameObject);
                return;
            }

            BossEnemy boss = collision.GetComponent<BossEnemy>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                PlayHitSound();
                Destroy(gameObject);
            }
        }
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

    private void OnBecameInvisible()
    {
        Destroy(gameObject, 0.5f);
    }
}