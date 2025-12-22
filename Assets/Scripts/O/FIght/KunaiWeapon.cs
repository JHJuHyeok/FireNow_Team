using System.Collections;
using UnityEngine;

public class KunaiWeapon : MonoBehaviour
{
    [Header("Kunai Settings")]
    [SerializeField] private GameObject kunaiPrefab;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float speed = 15f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private int projectileCount = 1;

    [Header("Target Settings")]
    [SerializeField] private float targetRange = 10f;
    [SerializeField] private LayerMask enemyLayer;

    private float nextFireTime = 0f;

    private void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Fire()
    {
        Enemy target = FindNearestEnemy();
        if (target == null) return;

        if (projectileCount == 1)
        {
            // 단발
            ShootKunai(target.transform.position);
        }
        else
        {
            // 다발 (부채꼴)
            float angleStep = 20f; // 쿠나이 간 각도
            float startAngle = -(angleStep * (projectileCount - 1)) / 2f;

            for (int i = 0; i < projectileCount; i++)
            {
                float angle = startAngle + (angleStep * i);
                Vector3 direction = Quaternion.Euler(0, 0, angle) * (target.transform.position - transform.position).normalized;
                ShootKunai(transform.position + direction * 0.5f, direction);
            }
        }
    }

    private void ShootKunai(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        ShootKunai(transform.position, direction);
    }

    private void ShootKunai(Vector3 position, Vector3 direction)
    {
        GameObject kunai = Instantiate(kunaiPrefab, position, Quaternion.identity);

        // 쿠나이 회전 (방향에 맞춰)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        kunai.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rb = kunai.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }

        // 데미지 설정 (Bullet 스크립트 사용)
        Bullet bullet = kunai.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(direction, damage, speed);
        }
    }

    private Enemy FindNearestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, targetRange, enemyLayer);
        Enemy nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var collider in enemies)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }

        return nearestEnemy;
    }

    // 레벨업 시 호출
    public void SetLevel(int level, AbilityLevelData levelData)
    {
        damage = levelData.damage;
        speed = levelData.speed;
        fireRate = levelData.cooldown;
        projectileCount = levelData.projectileCount;

        Debug.Log($"쿠나이 업그레이드 - 데미지: {damage}, 개수: {projectileCount}, 쿨다운: {fireRate}");
    }
}