using System.Collections;
using UnityEngine;

public class DrillWeapon : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject drillPrefab;

    [Header("Stats")]
    private float damageRate = 1f;
    private int projectileCount = 1;
    private float cooldown = 4f;
    private float speed = 1f;

    [Header("Spawn Settings")]
    private Transform player;
    private PlayerController playerController;
    private float sequentialDelay = 0.15f;
    private bool isActive = true;
    private bool isEvolved = false;

    [Header("Evolution")]
    private DrillProjectile trackingDrill; // 추적 드릴 참조

    [Header("Sound")]
    private string hitSoundName = "Drill";

    public void SetEvolution(bool evolved)
    {
        isEvolved = evolved;

        if (isEvolved)
        {
            // 진화 시 기존 모든 드릴 삭제
            DestroyAllExistingDrills();

            // 추적 드릴 하나만 생성
            CreateTrackingDrill();
        }
    }

    private void Start()
    {
        player = transform.parent;
        if (player == null) player = transform;

        playerController = player.GetComponent<PlayerController>();

        StartCoroutine(FireRoutine());
    }

    public void UpdateStats(AbilityLevelData levelData)
    {
        damageRate = levelData.damageRate;
        projectileCount = levelData.projectileCount;
        cooldown = levelData.cooldown;
        speed = levelData.speed;
    }

    private IEnumerator FireRoutine()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(cooldown);

            if (!isEvolved)
            {
                yield return StartCoroutine(FireDrillsSequentially());
            }
            // 진화 모드에서는 추적 드릴이 자동으로 동작
        }
    }

    // 기존 모든 드릴 삭제
    private void DestroyAllExistingDrills()
    {
        DrillProjectile[] existingDrills = FindObjectsOfType<DrillProjectile>();
        foreach (DrillProjectile drill in existingDrills)
        {
            if (drill != null)
            {
                Destroy(drill.gameObject);
            }
        }
    }

    // 추적 드릴 생성
    private void CreateTrackingDrill()
    {
        if (drillPrefab == null) return;

        float baseDamage = playerController != null ? playerController.GetAttackPower() : 10f;
        float finalDamage = baseDamage * damageRate;

        GameObject drill = Instantiate(drillPrefab, player.position, Quaternion.identity);
        trackingDrill = drill.GetComponent<DrillProjectile>();

        if (trackingDrill == null)
        {
            trackingDrill = drill.AddComponent<DrillProjectile>();
        }

        trackingDrill.SetEvolution(true);
        trackingDrill.SetHitSound(hitSoundName);

        // 추적 모드로 초기화
        trackingDrill.InitializeTracking(finalDamage, speed * 3f, player);
    }

    private IEnumerator FireDrillsSequentially()
    {
        if (drillPrefab == null)
        {
            yield break;
        }

        Enemy nearestEnemy = FindNearestEnemy();
        float baseDamage = playerController != null ? playerController.GetAttackPower() : 10f;
        float finalDamage = baseDamage * damageRate;

        for (int i = 0; i < projectileCount; i++)
        {
            float angleOffset = 0f;
            if (projectileCount > 1)
            {
                float totalSpread = 360f / projectileCount;
                angleOffset = i * totalSpread;
            }

            Vector2 direction;
            if (nearestEnemy != null)
            {
                direction = (nearestEnemy.transform.position - player.position).normalized;
            }
            else
            {
                float randomAngle = Random.Range(0f, 360f);
                direction = new Vector2(
                    Mathf.Cos(randomAngle * Mathf.Deg2Rad),
                    Mathf.Sin(randomAngle * Mathf.Deg2Rad)
                );
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle += angleOffset;
            float rad = angle * Mathf.Deg2Rad;
            direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            GameObject drill = Instantiate(drillPrefab, player.position, Quaternion.identity);
            DrillProjectile projectile = drill.GetComponent<DrillProjectile>();
            if (projectile == null)
            {
                projectile = drill.AddComponent<DrillProjectile>();
            }

            projectile.SetEvolution(false);
            projectile.Initialize(finalDamage, speed, direction);
            projectile.SetHitSound(hitSoundName);

            if (i < projectileCount - 1)
            {
                yield return new WaitForSeconds(sequentialDelay);
            }
        }
    }

    private Enemy FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Enemy nearest = null;
        float minDistance = float.MaxValue;

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = Vector2.Distance(player.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

    private void OnDestroy()
    {
        isActive = false;
        StopAllCoroutines();

        // 추적 드릴도 삭제
        if (trackingDrill != null)
        {
            Destroy(trackingDrill.gameObject);
        }
    }
}