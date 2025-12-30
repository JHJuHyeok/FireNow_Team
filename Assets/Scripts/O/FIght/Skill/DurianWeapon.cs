using UnityEngine;

public class DurianWeapon : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject durianPrefab;

    [Header("Stats")]
    private float damageRate = 1f;
    private int projectileCount = 1;
    private float cooldown = 1f;
    private float speed = 1f;

    [Header("Spawn Settings")]
    private Transform player;

    // 현재 활성화된 단 하나의 두리안
    private GameObject currentDurian;

    private void Start()
    {
        // 플레이어 찾기 개선
        player = transform.parent;
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                player = transform; // 최후의 수단
            }
        }

        // 처음 한 번 발사
        FireDurian();
    }

    public void UpdateStats(AbilityLevelData levelData)
    {
        damageRate = levelData.damageRate;
        projectileCount = levelData.projectileCount;
        cooldown = levelData.cooldown;
        speed = levelData.speed;

        // 기존 두리안 제거
        if (currentDurian != null)
        {
            Destroy(currentDurian);
            currentDurian = null;
        }

        // 새로 발사
        FireDurian();
    }

    private void FireDurian()
    {
        // 두리안 프리팹 체크
        if (durianPrefab == null)
        {

            return;
        }

        // 플레이어 체크 추가
        if (player == null)
        {
            // 다시 한번 플레이어 찾기 시도
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                return; // 플레이어를 못 찾으면 발사 중단
            }
        }

        Enemy nearestEnemy = FindNearestEnemy();
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

        currentDurian = Instantiate(durianPrefab, player.position, Quaternion.identity);
        DurianProjectile projectile = currentDurian.GetComponent<DurianProjectile>();

        if (projectile == null)
        {
            projectile = currentDurian.AddComponent<DurianProjectile>();
        }

        projectile.Initialize(damageRate, speed, direction);
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
        if (currentDurian != null)
        {
            Destroy(currentDurian);
        }
    }
}