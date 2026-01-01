using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite normalSprite;      // 일반 번개
    public Sprite evolutionSprite;   // 진화 번개

    [Header("Stats")]
    private float damage = 15f;
    private float range = 1f;

    [Header("Visual")]
    private float strikeDelay = 0.3f;
    private SpriteRenderer spriteRenderer;
    private bool hasStruck = false;
    private string hitSoundName; // 추가
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

    public void SetEvolution(bool isEvolution)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isEvolution ? evolutionSprite : normalSprite;
        }
    }
    public void SetHitSound(string soundName)
    {
        hitSoundName = soundName;
    }
    public void Initialize(float finalDamage, float strikeRange)
    {
        damage = finalDamage;
        range = strikeRange;

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

                BossEnemy boss = col.GetComponent<BossEnemy>();
                if (boss != null)
                {
                    boss.TakeDamage(damage);
                }
            }
        }

        if (collider != null)
        {
            collider.enabled = false;
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}