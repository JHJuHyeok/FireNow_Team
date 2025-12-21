using DG.Tweening;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    private float damage;
    private float speed;
    private float lifeTime;
    private int pierceCount;
    private int currentPierceCount = 0;

    private Vector2 direction;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Tween spinTween;

    [SerializeField] private float spinSpeed = 0.3f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(ShurikenData data, Vector2 dir)
    {
        damage = data.damage;
        speed = data.speed;
        lifeTime = data.lifeTime;
        pierceCount = data.pierceCount;
        direction = dir.normalized;
        currentPierceCount = 0;

        if (spriteRenderer != null && data.sprite != null)
        {
            spriteRenderer.sprite = data.sprite;
        }

        // 발사 방향으로 회전
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 이동
        rb.velocity = direction * speed;

        // 회전 애니메이션
        Spin();

        // 수명 관리
        Destroy(gameObject, lifeTime);
    }

    private void Spin()
    {
        spinTween?.Kill();
        spinTween = transform.DORotate(new Vector3(0, 0, 360), spinSpeed, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1)
            .SetRelative(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 적과 충돌
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                currentPierceCount++;

                // 관통 횟수 초과 시 파괴
                if (currentPierceCount >= pierceCount)
                {
                    DestroyShuriken();
                }
            }
        }
    }

    private void DestroyShuriken()
    {
        spinTween?.Kill();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        spinTween?.Kill();
    }
}