using UnityEngine;

public class DurianWeapon : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject durianPrefab;

    [Header("Stats")]
    private float damageRate = 1f;
    private int projectileCount = 1;
    private float cooldown = 4f;
    private float speed = 1f;
    private int currentLevel = 1; // 레벨 


    [Header("Spawn Settings")]
    private Transform player;
    private PlayerController playerController;
    private GameObject currentDurian;
    private bool isEvolved = false;
    [Header("Sound")]
    private string hitSoundName = "Durian";
    public void SetEvolution(bool evolved)
    {
        isEvolved = evolved;
    }

    private void Start()
    {
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
                player = transform;
            }
        }

        playerController = player.GetComponent<PlayerController>();
        FireDurian();
    }

    public void UpdateStats(AbilityLevelData levelData)
    {
        damageRate = levelData.damageRate;
        projectileCount = levelData.projectileCount;
        cooldown = levelData.cooldown;
        speed = levelData.speed;
        currentLevel++; // 레벨 증가
        if (currentDurian != null)
        {
            DurianProjectile projectile = currentDurian.GetComponent<DurianProjectile>();
            if (projectile != null)
            {
                float baseDamage = playerController != null ? playerController.GetAttackPower() : 12f;
                float finalDamage = baseDamage * damageRate;

                projectile.UpdateDamage(finalDamage);
                projectile.UpdateSpeed(speed);
                projectile.SetEvolution(isEvolved);
                // 레벨당 1.02배씩 증가
                //float scale = 1f + (currentLevel - 1) * 0.02f;
                //projectile.UpdateScale(scale);
            }
        }
        else
        {
            FireDurian();
        }
    }
    private void FireDurian()
    {
        if (durianPrefab == null)
        {
            return;
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                return;
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

        float baseDamage = playerController != null ? playerController.GetAttackPower() : 12f;
        float finalDamage = baseDamage * damageRate;

        currentDurian = Instantiate(durianPrefab, player.position, Quaternion.identity);
        DurianProjectile projectile = currentDurian.GetComponent<DurianProjectile>();
        if (projectile == null)
        {
            projectile = currentDurian.AddComponent<DurianProjectile>();
        }

        projectile.SetEvolution(isEvolved);
        projectile.Initialize(finalDamage, speed, direction);
        projectile.SetHitSound(hitSoundName); // 효과음
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