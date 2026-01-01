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
    private PlayerController contactPlayer;
    private bool isInContactWithPlayer;
    private float nextDotDamageTime;
    private int currentPattern = 0;
    private float patternTimer;

    // public - 이벤트는 외부 구독 필요
    public event Action OnBossDefeated;
    public event Action<float> OnHealthChanged;

    // public 프로퍼티 - 읽기 전용
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        patternTimer = patternChangeInterval;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        bossCollider = GetComponent<CircleCollider2D>();
        if (bossCollider == null)
        {
            bossCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        bossCollider.isTrigger = true;
        bossCollider.radius = 1f;
        bossCollider.enabled = true;

        gameObject.tag = "Enemy";

        contactPlayer = null;
        isInContactWithPlayer = false;

        OnHealthChanged?.Invoke(currentHealth);
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        patternTimer -= Time.deltaTime;
        if (patternTimer <= 0)
        {
            ChangePattern();
            patternTimer = patternChangeInterval;
        }

        ExecuteCurrentPattern(distanceToPlayer);

        if (isInContactWithPlayer && contactPlayer != null && Time.time >= nextDotDamageTime)
        {
            DealDamageToPlayer();
            nextDotDamageTime = Time.time + damageInterval;
        }
    }

    private void ExecuteCurrentPattern(float distanceToPlayer)
    {
        Vector2 moveDirection = Vector2.zero;

        if (distanceToPlayer > attackRange && distanceToPlayer <= chaseRange)
        {
            switch (currentPattern)
            {
                case 0:
                    moveDirection = (player.position - transform.position).normalized;
                    transform.position += (Vector3)moveDirection * moveSpeed * Time.deltaTime;
                    break;

                case 1:
                    moveDirection = (player.position - transform.position).normalized;
                    float zigzag = Mathf.Sin(Time.time * 3f) * 1.5f;
                    Vector2 perpendicular = new Vector2(-moveDirection.y, moveDirection.x);
                    Vector2 finalDirection = moveDirection + (perpendicular * zigzag);
                    transform.position += (Vector3)finalDirection.normalized * moveSpeed * Time.deltaTime;
                    break;

                case 2:
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

    private void OnTriggerEnter2D(Collider2D collision)
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

            if (player != null)
            {
                contactPlayer = player;
                isInContactWithPlayer = true;
                nextDotDamageTime = Time.time;
                DealDamageToPlayer();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInContactWithPlayer = true;
        }
    }

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

    private bool isDead = false; // 추가

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return; // 이미 죽었으면 데미지 무시

        currentHealth -= damageAmount;

        ShowDamageText(damageAmount);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0; // 0으로 고정
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return; // 중복 방지
        isDead = true;

   

        OnBossDefeated?.Invoke();

        if (KillCounter.Instance != null)
        {
            KillCounter.Instance.AddKill();
        }

        DropExperience(500);

        // Collider 비활성화
        if (bossCollider != null)
        {
            bossCollider.enabled = false;
        }

        Destroy(gameObject, 0.5f); // 약간 딜레이
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        if (bossCollider != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, bossCollider.radius);
        }
    }
}