using System.Collections;
using UnityEngine;

public class KunaiWeapon : MonoBehaviour
{
    [Header("Kunai Settings")]
    [SerializeField] private GameObject kunaiPrefab; // Bullet 프리팹
    private float damage = 10f;
    private float speed = 15f;
    private float fireRate = 1f;
    private int projectileCount = 1;

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
        // projectileCount만큼 쿠나이 발사
        for (int i = 0; i < projectileCount; i++)
        {
            Enemy target = FindNearestEnemy();
            if (target != null)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                SpawnBullet(direction);
            }
            else
            {
                break;
            }
        }
    }

    private void SpawnBullet(Vector3 direction)
    {

        if(kunaiPrefab == null)
        {
            return;
        }


        // Bullet 생성
        GameObject bullet = Instantiate(kunaiPrefab, transform.position, Quaternion.identity);

        // Bullet 스크립트 초기화
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(direction, damage, speed);
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

    // 레벨업 시 JSON 데이터 적용
    public void SetLevel(int level, AbilityLevelData levelData)
    {
        damage = levelData.damageRate;
        speed = levelData.speed;
        fireRate = levelData.cooldown;
        projectileCount = levelData.projectileCount;
    }
}