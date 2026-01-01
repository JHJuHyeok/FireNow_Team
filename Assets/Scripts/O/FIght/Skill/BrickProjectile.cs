using UnityEngine;

public class BrickProjectile : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite normalSprite;      // 일반 벽돌
    public Sprite evolutionSprite;   // 덤벨 (진화)

    private float damage;
    private float speed;
    private Vector2 direction;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private string hitSoundName; // 추가

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0;

        // SpriteRenderer 가져오기
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 진화 여부에 따라 스프라이트 설정
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
        speed = spd;
        direction = dir;
        rb.velocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // 히트 사운드 설정 메서드 추가
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
                PlayHitSound(); // 추가
                Destroy(gameObject);
                return;
            }

            BossEnemy boss = collision.GetComponent<BossEnemy>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                PlayHitSound(); // 추가
                Destroy(gameObject);
            }
        }
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

    private void OnBecameInvisible()
    {
        Destroy(gameObject, 0.5f);
    }
}