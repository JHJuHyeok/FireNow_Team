using UnityEngine;

public class RocketProjectile : MonoBehaviour
{

    [Header("Sprites")]
    public Sprite normalSprite;      // 일반
    public Sprite evolutionSprite;   // 진화 



    [Header("Stats")]
    private float damage = 20f;
    private float speed = 8f;
    private float explosionRange = 2f;
    private float lifetime = 5f;

    [Header("Explosion")]
    public GameObject explosionEffectPrefab;

    private string hitSoundName; // 추가


    private Vector2 direction;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
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
    public void SetHitSound(string soundName)
    {
        hitSoundName = soundName;
    }
    // 최종 데미지를 받도록 수정
    public void Initialize(float finalDamage, float projectileSpeed, float range, Vector2 dir)
    {
        damage = finalDamage; // 이미 계산된 최종 데미지
        speed = projectileSpeed * 6f;
        explosionRange = range;
        direction = dir.normalized;

        rb.velocity = direction * speed;

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
    public void SetEvolution(bool isEvolution)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isEvolution ? evolutionSprite : normalSprite;
        }
    }
    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

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

                BossEnemy boss = col.GetComponent<BossEnemy>();
                if (boss != null)
                {
                    boss.TakeDamage(damage);
                }
            }
        }

        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 1f);
        }

        Destroy(gameObject);
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}