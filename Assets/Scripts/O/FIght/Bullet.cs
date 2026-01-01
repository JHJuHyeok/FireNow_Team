using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Bullet;

public class Bullet : MonoBehaviour, IDamageDealer
{
    [Header("Sprites")]
    public Sprite normalSprite;      // 일반 쿠나이
    public Sprite evolutionSprite;   // 고스트 쿠나이 (진화)

    [Header("Bullet Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] public int boxDamage = 10;

    private Vector3 direction;
    private CircleCollider2D bulletCollider;
    private SpriteRenderer spriteRenderer;

    public interface IDamageDealer
    {
        float GetDamage();
    }

    private void Awake()
    {
        bulletCollider = GetComponent<CircleCollider2D>();
        if (bulletCollider == null)
        {
            bulletCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        bulletCollider.isTrigger = true;
        bulletCollider.radius = 0.2f;
        bulletCollider.enabled = true;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public float GetDamage()
    {
        return boxDamage;
    }

    public void SetEvolution(bool isEvolution)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isEvolution ? evolutionSprite : normalSprite;
        }
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector3 shootDirection, float bulletDamage, float bulletSpeed)
    {
        direction = shootDirection.normalized;
        damage = bulletDamage;
        speed = bulletSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 135f);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, bulletCollider.radius);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    Enemy enemy = hit.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                        Destroy(gameObject);
                        return;
                    }
                }

                if (hit.CompareTag("Box"))
                {
                    BreakableBox box = hit.GetComponent<BreakableBox>();
                    if (box != null)
                    {
                        box.Break();
                        Destroy(gameObject);
                        return;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            return;
        }

        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (bulletCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, bulletCollider.radius);
        }
    }
}