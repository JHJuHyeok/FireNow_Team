using UnityEngine;

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

    [Header("Enemy Push")]
    [SerializeField] private float pushRadius = 0.3f;
    [SerializeField] private float pushForce = 5f;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        // 조이스틱 입력 (고정 우선)
        Vector2 direction = Vector2.zero;
        if (fixedJoystick != null && fixedJoystick.Direction.magnitude > 0)
        {
            direction = fixedJoystick.Direction;
        }
        else if (dynamicJoystick != null && dynamicJoystick.Direction.magnitude > 0)
        {
            direction = dynamicJoystick.Direction;
        }

        // Z 좌표를 유지하면서 이동
        Vector3 movement = new Vector3(direction.x, direction.y, 0) * moveSpeed * Time.deltaTime;
        transform.position += movement;

        // Z 좌표 강제 고정 (2D 게임에서 필수!)
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;

        // 주변 적들 밀어내기
        PushAwayNearbyEnemies();
    }
    private void PushAwayNearbyEnemies()
    {
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

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //Debug.Log("Player Died!");
    }
}