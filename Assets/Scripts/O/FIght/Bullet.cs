using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 5f;

    private Vector3 direction;
    private CircleCollider2D bulletCollider;

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

      
    }

    // Bullet.cs의 Start에 추가
    private void Start()
    {

        Destroy(gameObject, lifetime);
    }
    public void Initialize(Vector3 shootDirection, float bulletDamage, float bulletSpeed)
    {
        direction = shootDirection.normalized;
        damage = bulletDamage;
        speed = bulletSpeed;

        // 방향 벡터를 각도로 변환
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 스프라이트의 초기 각도에 맞춰 보정

        transform.rotation = Quaternion.Euler(0, 0, angle -135f);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
    private void FixedUpdate()
    {
        // 근처의 모든 Collider 찾기
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

    // Scene 뷰에서 Collider 범위 시각화
    private void OnDrawGizmos()
    {
        if (bulletCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, bulletCollider.radius);
        }
    }
}