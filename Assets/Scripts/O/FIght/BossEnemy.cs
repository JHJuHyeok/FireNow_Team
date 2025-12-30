using System;
using System.Collections;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    [Header("보스 스탯")]
    [SerializeField] private float maxHealth = 1000f;
    [SerializeField] private float moveSpeed = 3f;

    [Header("데미지 설정")]
    [SerializeField] private int damage = 20;
    [SerializeField] private float damageInterval = 0.5f;

    [Header("이동 설정")]
    [SerializeField] private float chaseRange = 15f;
    [SerializeField] private float attackRange = 1f;

    [Header("패턴 설정")]
    [SerializeField] private float patternChangeInterval = 10f;

    private float currentHealth;
    private Transform player;
    private CircleCollider2D bossCollider;

    // DOT 데미지용
    private PlayerController contactPlayer;
    private bool isInContactWithPlayer;
    private float nextDotDamageTime;

    // 패턴 관리
    private int currentPattern = 0;
    private float patternTimer;

    public event Action OnBossDefeated;

    void Start()
    {
        currentHealth = maxHealth;
        patternTimer = patternChangeInterval;

        // 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Collider 설정
        bossCollider = GetComponent<CircleCollider2D>();
        if (bossCollider == null)
        {
            bossCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        bossCollider.isTrigger = true;
        bossCollider.radius = 1f; // 보스는 크기가 크므로
        bossCollider.enabled = true;

        // 태그 설정
        gameObject.tag = "Enemy";

        // 초기화
        contactPlayer = null;
        isInContactWithPlayer = false;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 패턴 변경 타이머
        patternTimer -= Time.deltaTime;
        if (patternTimer <= 0)
        {
            ChangePattern();
            patternTimer = patternChangeInterval;
        }

        // 현재 패턴에 따른 이동
        ExecuteCurrentPattern(distanceToPlayer);

        // 플레이어와 접촉 중일 때 DOT 데미지
        if (isInContactWithPlayer && contactPlayer != null && Time.time >= nextDotDamageTime)
        {
            DealDamageToPlayer();
            nextDotDamageTime = Time.time + damageInterval;
        }
    }

    private void ExecuteCurrentPattern(float distanceToPlayer)
    {
        Vector2 moveDirection = Vector2.zero;

        // 공격 범위 밖에 있을 때만 이동
        if (distanceToPlayer > attackRange && distanceToPlayer <= chaseRange)
        {
            switch (currentPattern)
            {
                case 0: // 직선 추적
                    moveDirection = (player.position - transform.position).normalized;
                    transform.position += (Vector3)moveDirection * moveSpeed * Time.deltaTime;
                    break;

                case 1: // 지그재그 추적
                    moveDirection = (player.position - transform.position).normalized;
                    float zigzag = Mathf.Sin(Time.time * 3f) * 1.5f;
                    Vector2 perpendicular = new Vector2(-moveDirection.y, moveDirection.x);
                    Vector2 finalDirection = moveDirection + (perpendicular * zigzag);
                    transform.position += (Vector3)finalDirection.normalized * moveSpeed * Time.deltaTime;
                    break;

                case 2: // 빠른 돌진
                    moveDirection = (player.position - transform.position).normalized;
                    transform.position += (Vector3)moveDirection * (moveSpeed * 1.5f) * Time.deltaTime;
                    break;
            }
        }
    }

    private void ChangePattern()
    {
        currentPattern = (currentPattern + 1) % 3;

    }

    // Trigger 충돌 시작 (플레이어 감지)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 먼저 충돌한 오브젝트에서 찾기
            PlayerController player = collision.GetComponent<PlayerController>();

            // 없으면 부모에서 찾기
            if (player == null)
            {
                player = collision.GetComponentInParent<PlayerController>();
            }

            // 없으면 자식에서 찾기
            if (player == null)
            {
                player = collision.GetComponentInChildren<PlayerController>();
            }

            if (player != null)
            {
                contactPlayer = player;
                isInContactWithPlayer = true;
                nextDotDamageTime = Time.time;
                DealDamageToPlayer(); // 즉시 1회 데미지
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
            if (player == null)
            {
                player = collision.GetComponentInParent<PlayerController>();
            }
            if (player == null)
            {
                player = collision.GetComponentInChildren<PlayerController>();
            }

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

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        // 데미지 텍스트 표시
        ShowDamageText(damageAmount);

   

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
        // 보스 처치 이벤트 발동
        OnBossDefeated?.Invoke();

        // 킬 카운트 증가
        if (KillCounter.Instance != null)
        {
            KillCounter.Instance.AddKill();
        }

        // 경험치 드롭 (보스는 많이)
        DropExperience(500); // 보스 경험치

        Destroy(gameObject);
    }

    private void DropExperience(int expAmount)
    {
        GameObject expOrbPrefab = Resources.Load<GameObject>("Prefabs/ExpOrb");
        if (expOrbPrefab != null)
        {
            Vector3 dropPosition = transform.position;
            GameObject expOrb = Instantiate(expOrbPrefab, dropPosition, Quaternion.identity);

            ExpOrb orbScript = expOrb.GetComponent<ExpOrb>();
            if (orbScript != null)
            {
                orbScript.SetExpAmount(expAmount);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // 공격 범위 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 추적 범위 시각화
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // Collider 시각화
        if (bossCollider != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, bossCollider.radius);
        }
    }
}