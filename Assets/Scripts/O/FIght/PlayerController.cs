using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Joystick")]
    [SerializeField] private Joystick fixedJoystick;
    [SerializeField] private DynamicJoystick dynamicJoystick;

    [Header("Initial Stats")]
    [SerializeField] private float initialMaxHP = 1000f;
    [SerializeField] private float initialAttack = 20f;
    [SerializeField] private float initialDefence = 0f;
    [SerializeField] private float initialMeatHeal = 30f;

    private BattleStat battleStat;
    private float currentHealth;

    [Header("UI")]
    [SerializeField] private HPBar hpBar;

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
    private float minX = -100f;  
    private float maxX = 100f;   

    private const string ANIM_IS_MOVING = "isMoving";
    private const string ANIM_MOVE_X = "moveX";
    private const string ANIM_MOVE_Y = "moveY";
    private const string ANIM_DIE = "Die";

    private Vector2 lastMoveDirection = Vector2.down;

    public BattleStat GetBattleStat() => battleStat;

    // 기존 메서드 (Y축만)
    //public void SetMovementBounds(float min, float max)
    //{
    //    hasMovementBounds = true;
    //    minY = min;
    //    maxY = max;
    //}

    // 새로운 메서드 (X축, Y축 모두)
    public void SetMovementBounds(float minY, float maxY, float minX, float maxX)
    {
        hasMovementBounds = true;
        this.minY = minY;
        this.maxY = maxY;
        this.minX = minX;
        this.maxX = maxX;
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
            pos.x = Mathf.Clamp(pos.x, minX, maxX);  // X축 제한 추가
            transform.position = pos;
        }
    }

    private void Start()
    {
        InitializeStats();
        InitializeHealth();
        InitializeComponents();
    }

    private void InitializeStats()
    {
        battleStat = new BattleStat();
        battleStat.ClearRuntimeStats();

        battleStat.maxHP.baseValue = initialMaxHP;
        battleStat.attack.baseValue = initialAttack;
        battleStat.defence.baseValue = initialDefence;
        battleStat.getHPWithMeat.baseValue = initialMeatHeal;
        battleStat.moveSpeed.baseValue = moveSpeed;

        battleStat.Refresh();
    }

    private void InitializeHealth()
    {
        currentHealth = battleStat.finalMaxHP;

        if (hpBar == null)
        {
            hpBar = FindObjectOfType<HPBar>();
        }

        if (hpBar != null)
        {
            hpBar.maxHP = battleStat.finalMaxHP;
            hpBar.SetHP(currentHealth, battleStat.finalMaxHP);
        }
    }

    private void InitializeComponents()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (isDead) return;

        Vector2 direction = GetMovementDirection();
        bool isMoving = direction.magnitude > 0.01f;

        if (isMoving)
        {
            float currentMoveSpeed = battleStat.finalMoveSpeed;
            Vector3 movement = new Vector3(direction.x, direction.y, 0) * currentMoveSpeed * Time.deltaTime;
            transform.position += movement;
            lastMoveDirection = direction.normalized;
        }

        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;

        UpdateAnimation(isMoving, direction);
        PushAwayNearbyEnemies();
    }

    private Vector2 GetMovementDirection()
    {
        Vector2 direction = Vector2.zero;
        if (fixedJoystick != null && fixedJoystick.Direction.magnitude > 0)
        {
            direction = fixedJoystick.Direction;
        }
        else if (dynamicJoystick != null && dynamicJoystick.Direction.magnitude > 0)
        {
            direction = dynamicJoystick.Direction;
        }
        return direction;
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

    public void TakeDamage(float rawDamage)
    {
        if (isDead) return;

        float actualDamage = CalculateDamage(rawDamage, battleStat.finalDefence);
        currentHealth -= actualDamage;
        currentHealth = Mathf.Max(0, currentHealth);

        if (hpBar != null)
        {
            hpBar.TakeDamage(actualDamage);
        }

        if (currentHealth <= 0)
        {
            hpBar.Die();
        }
    }

    private float CalculateDamage(float rawDamage, float defence)
    {
        float damageReduction = 100f / (100f + defence);
        float actualDamage = rawDamage * damageReduction;
        return Mathf.Max(1f, actualDamage);
    }

    public void Heal(float baseAmount)
    {
        if (isDead) return;

        float maxHP = battleStat != null ? battleStat.finalMaxHP : initialMaxHP;
        float baseHeal = maxHP * 0.3f;
        float bonusHeal = battleStat != null ? battleStat.finalGetHP : 0f;
        float actualHealAmount = baseHeal + bonusHeal;

        currentHealth += actualHealAmount;
        currentHealth = Mathf.Min(currentHealth, maxHP);

        if (hpBar != null)
        {
            hpBar.Heal(actualHealAmount);
        }
    }

    public float GetAttackPower()
    {
        return battleStat.finalAttack;
    }

    public void RefreshStats()
    {
        battleStat.Refresh();

        if (hpBar != null)
        {
            hpBar.maxHP = battleStat.finalMaxHP;
            hpBar.SetHP(currentHealth, battleStat.finalMaxHP);
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger(ANIM_DIE);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pushRadius);
    }
}