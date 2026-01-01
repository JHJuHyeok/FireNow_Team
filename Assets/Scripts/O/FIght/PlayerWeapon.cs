using System.Collections;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float damageRate = 1f;
    [SerializeField] private float bulletSpeed = 15f;
    [SerializeField] private float fireRate = 4f;
    [SerializeField] private int bulletCount = 1;
    [SerializeField] private float delayBetweenBullets = 0.1f;

    [Header("Auto Target Settings")]
    [SerializeField] private bool autoTarget = true;
    [SerializeField] private float targetRange = 10f;
    [SerializeField] private LayerMask enemyLayer;

    private float nextFireTime = 0f;
    private bool isFiring = false;
    private bool isEvolved = false;
    private PlayerController playerController;


    [Header("Sound")]
    private string hitSoundName = "Kunai";

    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
    }

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
    public void SetWeaponLevel(AbilityLevelData levelData)
    {
        damageRate = levelData.damageRate;
        bulletSpeed = levelData.speed * 8;
        fireRate = levelData.cooldown > 0 ? levelData.cooldown : 1f;
        bulletCount = levelData.projectileCount;

        //Debug.Log($"[PlayerWeapon] SetWeaponLevel 호출됨!");
        //Debug.Log($"  damageRate: {damageRate}");
        //Debug.Log($"  bulletSpeed: {bulletSpeed}");
        //Debug.Log($"  fireRate: {fireRate}");
        //Debug.Log($"  bulletCount: {bulletCount}");
    }

    private void Shoot(Vector3 direction)
    {
        if (bulletPrefab == null || firePoint == null) return;

        float baseDamage = playerController != null ? playerController.GetAttackPower() : 10f;
        float finalDamage = baseDamage * damageRate;

        //Debug.Log($"[PlayerWeapon] Shoot 호출됨!");
        //Debug.Log($"  baseDamage (플레이어 공격력): {baseDamage}");
        //Debug.Log($"  damageRate (쿠나이 배율): {damageRate}");
        //Debug.Log($"  finalDamage (최종 데미지): {finalDamage}");

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetEvolution(isEvolved);
            bulletScript.Initialize(direction, finalDamage, bulletSpeed);
            bulletScript.SetHitSound(hitSoundName); // 효과음
            if (isEvolved)
            {
                bullet.transform.localScale *= 1.5f;
            }
        }
    }
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

            if (i < bulletCount - 1)
            {
                yield return new WaitForSeconds(delayBetweenBullets);
            }
        }

        nextFireTime = Time.time + fireRate;
        isFiring = false;
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

    public void SetWeaponEvolution(AbilityLevelData levelData, bool evolved)
    {
        isEvolved = evolved;
        SetWeaponLevel(levelData);
    }
}