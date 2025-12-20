using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint; // 총알 발사 위치
    [SerializeField] private float damage = 10f;
    [SerializeField] private float bulletSpeed = 15f;
    [SerializeField] private float fireRate = 0.5f; // 발사 간격

    [Header("Auto Target Settings")]
    [SerializeField] private bool autoTarget = true; // 자동 조준
    [SerializeField] private float targetRange = 10f; // 타겟 탐지 범위
    [SerializeField] private LayerMask enemyLayer; // 적 레이어

    private float nextFireTime = 0f;

    private void Update()
    {
        // 자동 발사
        if (Time.time >= nextFireTime)
        {
            if (autoTarget)
            {
                AutoShoot();
            }
        }
    }


    /// 자동 조준 및 발사

    private void AutoShoot()
    {
        Enemy nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            Vector3 direction = (nearestEnemy.transform.position - firePoint.position).normalized;
            Shoot(direction);
            nextFireTime = Time.time + fireRate;
        }
    }


    ///// 수동 발사 (마우스 방향)

    //private void ManualShoot()
    //{
    //    if (Input.GetMouseButton(0)) // 마우스 왼쪽 클릭
    //    {
    //        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        mousePos.z = 0f;
    //        Vector3 direction = (mousePos - firePoint.position).normalized;

    //        Shoot(direction);
    //        nextFireTime = Time.time + fireRate;
    //    }
    //}


    /// 탄환 발사

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


    /// 가장 가까운 적 찾기

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

    // 디버그용: 탐지 범위 표시
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, targetRange);
    //}
}