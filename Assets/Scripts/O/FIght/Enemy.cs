using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float attackRange = 0.5f;

    [Header("데미지 설정")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float damageInterval = 1f;

    [Header("충돌 설정")]
    [SerializeField] private float separationRadius = 0.5f;
    [SerializeField] private float separationForce = 2f;

    private EnemyData data;
    private Transform player;
    private float currentHealth;

    // DOT 데미지용
    private PlayerController contactPlayer;
    private bool isInContactWithPlayer;
    private float nextDotDamageTime;

    private CircleCollider2D enemyCollider;

    private static List<Enemy> allEnemies = new List<Enemy>();

    public void Initialize(EnemyData enemyData)
    {
        data = enemyData;
        currentHealth = data.hp;

        // 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Collider 설정
        enemyCollider = GetComponent<CircleCollider2D>();
        if (enemyCollider == null)
        {
            enemyCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        enemyCollider.isTrigger = true;
        enemyCollider.radius = 0.5f; // 일단 크게
        enemyCollider.enabled = true; // 명시적으로 활성화

        // 태그 설정
        gameObject.tag = "Enemy";
        // 초기화
        contactPlayer = null;
        isInContactWithPlayer = false;

        // 적 리스트에 추가
        allEnemies.Add(this);
    }

    private void OnDestroy()
    {
        // 적 리스트에서 제거
        allEnemies.Remove(this);
    }

    private void Update()
    {
        if (player == null || data == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        Vector2 moveDirection = Vector2.zero;

        // 플레이어 추적 (공격 범위 밖에 있을 때만)
        if (distanceToPlayer > attackRange && distanceToPlayer <= chaseRange)
        {
            moveDirection = (player.position - transform.position).normalized;
        }

        // 적끼리 분리 (커스텀 충돌)
        Vector2 separationDirection = CalculateSeparation();

        // 최종 이동 방향 = 플레이어 방향 + 분리 방향
        Vector2 finalDirection = (moveDirection + separationDirection).normalized;

        // 이동 (Transform 직접 조작)
        if (moveDirection != Vector2.zero)
        {
            transform.position += (Vector3)finalDirection * data.speed * Time.deltaTime;
        }
        else if (separationDirection != Vector2.zero)
        {
            // 공격 범위에 있어도 다른 적들한테 밀림
            transform.position += (Vector3)separationDirection * separationForce * Time.deltaTime;
        }

        // 플레이어와 접촉 중일 때 DOT 데미지
        if (isInContactWithPlayer && contactPlayer != null && Time.time >= nextDotDamageTime)
        {
            DealDamageToPlayer();
            nextDotDamageTime = Time.time + damageInterval;
        }
    }

    // 주변 적들과 분리되는 방향 계산
    private Vector2 CalculateSeparation()
    {
        Vector2 separationDirection = Vector2.zero;
        int nearbyCount = 0;

        foreach (Enemy otherEnemy in allEnemies)
        {
            if (otherEnemy == this || otherEnemy == null) continue;

            float distance = Vector2.Distance(transform.position, otherEnemy.transform.position);

            // 너무 가까우면 밀어냄
            if (distance < separationRadius && distance > 0.01f)
            {
                Vector2 pushDirection = (transform.position - otherEnemy.transform.position).normalized;
                // 거리가 가까울수록 더 강하게 밀어냄
                float pushStrength = 1f - (distance / separationRadius);
                separationDirection += pushDirection * pushStrength;
                nearbyCount++;
            }
        }

        if (nearbyCount > 0)
        {
            separationDirection /= nearbyCount;
            separationDirection = separationDirection.normalized * separationForce;
        }

        return separationDirection;
    }

    // Trigger 충돌 시작 (플레이어 감지)
    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                contactPlayer = player;
                isInContactWithPlayer = true;
                nextDotDamageTime = Time.time; // 즉시 첫 데미지
            }
        }
    }

    // Trigger 충돌 지속
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInContactWithPlayer = true;
        }
    }

    // Trigger 충돌 종료
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (contactPlayer == player)
            {
                isInContactWithPlayer = false;
                contactPlayer = null;
            }
        }
    }

    private void DealDamageToPlayer()
    {
        if (contactPlayer != null)
        {
            contactPlayer.TakeDamage(damage);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // 데미지 텍스트 표시
        ShowDamageText(damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ShowDamageText(float damage)
    {

        GameObject damageTextPrefab = Resources.Load<GameObject>("Prefabs/Enemies/DamageText");

        if (damageTextPrefab != null)
        {

            Vector3 spawnPosition = transform.position + new Vector3(0.5f, 0.5f, 0f);

            GameObject damageTextObj = Instantiate(damageTextPrefab);
   

            DamageText damageText = damageTextObj.GetComponent<DamageText>();
            if (damageText != null)
            {
           
                damageText.Initialize(damage, spawnPosition);
            }
        
        }
      
    }
    private void Die()
    {
        // 킬 카운트 증가
        if (KillCounter.Instance != null)
        {
            KillCounter.Instance.AddKill();
        }

        // 경험치 드롭
        if (!string.IsNullOrEmpty(data.dropItem))
        {
            DropExperience();
        }

        Destroy(gameObject);
    }

    private void DropExperience()
    {
        if (data.dropItem.StartsWith("exp_"))
        {
            string expString = data.dropItem.Replace("exp_", "");
            if (int.TryParse(expString, out int expAmount))
            {
                GameObject expOrbPrefab = Resources.Load<GameObject>("Prefabs/ExpOrb");
                if (expOrbPrefab != null)
                {
                    // 적이 죽은 위치에 생성 (랜덤 오프셋 추가 가능)
                    Vector3 dropPosition = transform.position;

                    GameObject expOrb = Instantiate(expOrbPrefab, dropPosition, Quaternion.identity);
                    ExpOrb orbScript = expOrb.GetComponent<ExpOrb>();
                    if (orbScript != null)
                    {
                        orbScript.SetExpAmount(expAmount);
                    }

           
                }
              
            }
        }
    }

    // Enemy.cs의 맨 아래에 추가
    private void OnDrawGizmos()
    {
        if (enemyCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyCollider.radius);
        }
    }

}