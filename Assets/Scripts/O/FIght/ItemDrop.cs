using UnityEngine;

public enum ItemType
{
    Money,      // 돈무더기
    Magnet,     // 자석
    Bomb,       // 폭탄
    Meat        // 고기
}

public class ItemDrop : MonoBehaviour
{
    [Header("아이템 설정")]
    public ItemType itemType;

    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float attractionRange = 5f;
    [SerializeField] private float colliderRadius = 0.5f; // Collider 크기

    private Transform player;
    private bool isBeingCollected = false;
    private CircleCollider2D itemCollider; // 추가

    void Start()
    {
        // Collider 설정 추가
        itemCollider = GetComponent<CircleCollider2D>();
        if (itemCollider == null)
        {
            itemCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        itemCollider.isTrigger = true;
        itemCollider.radius = colliderRadius;

        // 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attractionRange || isBeingCollected)
        {
            isBeingCollected = true;
            MoveToPlayer();
        }
    }

    private void MoveToPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CollectItem();
        }
    }

    private void CollectItem()
    {
        if (ItemManager.Instance != null)
        {
            ItemManager.Instance.OnItemCollected(itemType);
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attractionRange);

        // Collider 범위도 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, colliderRadius);
    }
}