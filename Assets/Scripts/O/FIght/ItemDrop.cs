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
    [SerializeField] private float attractionRange = 5f; // 자동으로 플레이어에게 끌리는 범위

    private Transform player;
    private bool isBeingCollected = false;

    void Start()
    {
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

        // 플레이어가 가까우면 자동으로 끌림
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
        // ItemManager에게 아이템 획득 알림
        if (ItemManager.Instance != null)
        {
            ItemManager.Instance.OnItemCollected(itemType);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        // 끌림 범위 시각화
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attractionRange);
    }
}