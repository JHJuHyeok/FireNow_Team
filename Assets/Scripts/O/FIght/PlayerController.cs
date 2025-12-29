using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Joystick")]
    [SerializeField] private Joystick fixedJoystick;
    [SerializeField] private DynamicJoystick dynamicJoystick;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private HPBar hpBar; // Slider 대신 HPBar 사용

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private bool isDead = false;

    [Header("Enemy Push")]
    [SerializeField] private float pushRadius = 0.3f;
    [SerializeField] private float pushForce = 5f;

    [Header("보스전 이동 제한")]
    private bool hasMovementBounds = false;
    private float minY = -100f;
    private float maxY = 100f;

    // 애니메이션 파라미터
    private const string ANIM_IS_MOVING = "isMoving";
    private const string ANIM_MOVE_X = "moveX";
    private const string ANIM_MOVE_Y = "moveY";
    private const string ANIM_DIE = "Die";

    private Vector2 lastMoveDirection = Vector2.down;

    public void SetMovementBounds(float min, float max)
    {
        hasMovementBounds = true;
        minY = min;
        maxY = max;
    }

    public void RemoveMovementBounds()
    {
        hasMovementBounds = false;
    }

    void LateUpdate()
    {
        if (hasMovementBounds)
        {
            Vector3 pos = transform.position;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            transform.position = pos;
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;

        // HPBar 자동 찾기
        if (hpBar == null)
        {
            hpBar = FindObjectOfType<HPBar>();
        }

        // HPBar 초기화
        if (hpBar != null)
        {
            hpBar.maxHP = maxHealth;
            hpBar.SetHP(currentHealth, maxHealth);
        }

        // Animator 자동 찾기
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (isDead) return;

        // 조이스틱 입력
        Vector2 direction = Vector2.zero;
        if (fixedJoystick != null && fixedJoystick.Direction.magnitude > 0)
        {
            direction = fixedJoystick.Direction;
        }
        else if (dynamicJoystick != null && dynamicJoystick.Direction.magnitude > 0)
        {
            direction = dynamicJoystick.Direction;
        }

        // 이동 처리
        bool isMoving = direction.magnitude > 0.01f;

        if (isMoving)
        {
            Vector3 movement = new Vector3(direction.x, direction.y, 0) * moveSpeed * Time.deltaTime;
            transform.position += movement;
            lastMoveDirection = direction.normalized;
        }

        // Z 좌표 고정
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;

        // 애니메이션 업데이트
        UpdateAnimation(isMoving, direction);

        // 적 밀어내기
        PushAwayNearbyEnemies();
    }

    private void UpdateAnimation(bool isMoving, Vector2 direction)
    {
        if (animator == null) return;

        animator.SetBool(ANIM_IS_MOVING, isMoving);

        if (isMoving)
        {
            animator.SetFloat(ANIM_MOVE_X, direction.x);
            animator.SetFloat(ANIM_MOVE_Y, direction.y);
        }
        else
        {
            animator.SetFloat(ANIM_MOVE_X, lastMoveDirection.x);
            animator.SetFloat(ANIM_MOVE_Y, lastMoveDirection.y);
        }
    }

    private void PushAwayNearbyEnemies()
    {
        if (isDead) return;

        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, pushRadius);
        foreach (Collider2D enemyCollider in nearbyEnemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Vector2 pushDirection = (enemyCollider.transform.position - transform.position).normalized;
                    float distance = Vector2.Distance(transform.position, enemyCollider.transform.position);
                    if (distance < pushRadius && distance > 0.01f)
                    {
                        float pushStrength = (1f - (distance / pushRadius)) * pushForce;
                        enemyCollider.transform.position += (Vector3)pushDirection * pushStrength * Time.deltaTime;
                    }
                }
            }
        }
    }

    // HPBar를 통해 데미지 받기
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        // HPBar 업데이트
        if (hpBar != null)
        {
            hpBar.TakeDamage(damage);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 체력 회복
    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        // HPBar 업데이트
        if (hpBar != null)
        {
            hpBar.Heal(amount);
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        // 사망 애니메이션
        if (animator != null)
        {
            animator.SetTrigger(ANIM_DIE);
        }

        //Debug.Log("Player Died!");

        // HPBar의 Die()가 자동으로 failUI 활성화
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pushRadius);
    }
}