using System.Collections;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float bulletSpeed = 15f;
    [SerializeField] private float fireRate = 1f; // 연발 후 대기 시간
    [SerializeField] private int bulletCount = 1;
    [SerializeField] private float delayBetweenBullets = 0.1f; // 쿠나이 간 딜레이

    [Header("Auto Target Settings")]
    [SerializeField] private bool autoTarget = true;
    [SerializeField] private float targetRange = 10f;
    [SerializeField] private LayerMask enemyLayer;

    private float nextFireTime = 0f;
    private bool isFiring = false;

    private void Update()
    {
        if (Time.time >= nextFireTime && !isFiring)
        {
            if (autoTarget)
            {
                StartCoroutine(AutoShoot());
            }
        }
    }

    // 연속 발사
    private IEnumerator AutoShoot()
    {
        isFiring = true;

        for (int i = 0; i < bulletCount; i++)
        {
            Enemy nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                Vector3 direction = (nearestEnemy.transform.position - firePoint.position).normalized;
                Shoot(direction);
            }

            // 쿠나이 사이 딜레이
            if (i < bulletCount - 1) // 마지막 쿠나이 후에는 딜레이 안줌
            {
                yield return new WaitForSeconds(delayBetweenBullets);
            }
        }

        // 다음 연발까지 대기
        nextFireTime = Time.time + fireRate;
        isFiring = false;
    }

    private void Shoot(Vector3 direction)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(direction, damage, bulletSpeed);
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

    public void SetWeaponLevel(AbilityLevelData levelData)
    {
        damage = levelData.damageRate;
        bulletSpeed = levelData.speed;
        fireRate = levelData.cooldown > 0 ? levelData.cooldown : 1f;
        bulletCount = levelData.projectileCount;

        Debug.Log($"쿠나이 설정 - 쿨타운: {fireRate}초, 개수: {bulletCount}, 연사 간격: {delayBetweenBullets}초");
    }
}