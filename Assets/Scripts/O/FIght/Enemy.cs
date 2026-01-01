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
    [SerializeField] private float separationRadius = 0.2f;
    [SerializeField] private float separationForce = 2f;

    [Header("사망 설정")]
    [SerializeField] private float deathDelay = 0.5f; // 사망 후 사라지기까지 딜레이
    [SerializeField] private string deathAnimationName = "Death"; // 사망 애니메이션 이름

    private EnemyData data;
    private Transform player;
    private float currentHealth;

    // DOT 데미지용
    private PlayerController contactPlayer;
    private bool isInContactWithPlayer;
    private float nextDotDamageTime;

    private CircleCollider2D enemyCollider;
    private Animator animator;
    private bool isDying = false;

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
        enemyCollider.radius = 0.3f; // 0.5f에서 0.3f로 줄임
        enemyCollider.enabled = true;

        // Animator 가져오기
        animator = GetComponent<Animator>();

        // 태그 설정
        gameObject.tag = "Enemy";

        // 초기화
        contactPlayer = null;
        isInContactWithPlayer = false;
        isDying = false;

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
        // 죽는 중이면 이동하지 않음
        if (isDying) return;

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
            transform.position += (Vector3)separationDirection * separationForce * Time.deltaTime;
        }

        // 플레이어와 접촉 중일 때 DOT 데미지
        if (isInContactWithPlayer && contactPlayer != null && Time.time >= nextDotDamageTime)
        {
            DealDamageToPlayer();
            nextDotDamageTime = Time.time + damageInterval;
        }
    }

    private Vector2 CalculateSeparation()
    {
        Vector2 separationDirection = Vector2.zero;
        int nearbyCount = 0;

        foreach (Enemy otherEnemy in allEnemies)
        {
            if (otherEnemy == this || otherEnemy == null) continue;

            float distance = Vector2.Distance(transform.position, otherEnemy.transform.position);

            if (distance < separationRadius && distance > 0.01f)
            {
                Vector2 pushDirection = (transform.position - otherEnemy.transform.position).normalized;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (isDying)
        {
            return;
        }

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

            }
           
            
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDying) return;

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
        if (isDying) return; // 이미 죽는 중이면 데미지 무시

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

    public void Die()
    {
        if (isDying) return; // 중복 호출 방지
        isDying = true;




        //if (KillCounter.Instance != null)
        //{
        //    KillCounter.Instance.AddKill();
        //}

        // 경험치 드롭
        if (!string.IsNullOrEmpty(data.dropItem))
        {
            DropExperience();
        }
      

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

        // Collider 비활성화 (더 이상 충돌 안 함)
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        // 사망 애니메이션 재생
        if (animator != null && !string.IsNullOrEmpty(deathAnimationName))
        {
            animator.SetTrigger(deathAnimationName);
        }

        // 딜레이 후 파괴
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        // 사망 애니메이션 재생 시간만큼 대기
        yield return new WaitForSeconds(deathDelay);

        // 오브젝트 파괴
        Destroy(gameObject);
    }

 

    private void DropExperience()
    {


        if (string.IsNullOrEmpty(data.dropItem) || !data.dropItem.StartsWith("exp_"))
        {
         
            return;
        }

        string expType = data.dropItem.Replace("exp_", "");


        GameObject expOrbPrefab = Resources.Load<GameObject>("Prefabs/ExpOrb");
        if (expOrbPrefab == null)
        {
          
            return;
        }

        Vector3 dropPosition = transform.position;
        GameObject expOrb = Instantiate(expOrbPrefab, dropPosition, Quaternion.identity);
 
        ExpOrb orbScript = expOrb.GetComponent<ExpOrb>();
        if (orbScript != null)
        {
            orbScript.SetExpType(expType);
          
        }
     
    }
    private void OnDrawGizmos()
    {
        if (enemyCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyCollider.radius);
        }
    }
}