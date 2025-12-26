using UnityEngine;
using static Bullet;

public class BreakableBox : MonoBehaviour
{
    [Header("아이템 드롭 설정")]
    [SerializeField] private GameObject[] itemPrefabs; // 4가지 아이템 프리팹
    [SerializeField] private float itemDropChance = 1.0f; 

    [Header("이펙트")]
    [SerializeField] private GameObject breakEffectPrefab;
    [Header("디버그")]
    [SerializeField] private bool showDebugLogs = true; // 디버그 로그 활성화

    private CircleCollider2D boxCollider;
    private bool isBreaking = false;

    void Start()
    {
        // Collider 설정
        boxCollider = GetComponent<CircleCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        boxCollider.isTrigger = true;
        boxCollider.radius = 0.5f;

        // 태그 설정
        gameObject.tag = "Box";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[Box] 충돌 감지! 오브젝트: {collision.gameObject.name}, 태그: {collision.tag}, 레이어: {LayerMask.LayerToName(collision.gameObject.layer)}");
        }

        if (isBreaking)
        {
            if (showDebugLogs)
            {
                Debug.Log($"[Box] 이미 파괴 중이라 무시");
            }
            return;
        }

        // 플레이어와 적은 무시
        if (collision.CompareTag("Player"))
        {
            if (showDebugLogs)
            {
                Debug.Log($"[Box] Player와 충돌 - 무시");
            }
            return;
        }

        if (collision.CompareTag("Enemy"))
        {
            if (showDebugLogs)
            {
                Debug.Log($"[Box] Enemy와 충돌 - 무시");
            }
            return;
        }

        // 나머지는 모두 박스를 부숨
        if (showDebugLogs)
        {
            Debug.Log($"[Box] 파괴 조건 충족! 부서짐");
        }
        Break();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[Box] 지속 충돌 중: {collision.gameObject.name}");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[Box] 충돌 종료: {collision.gameObject.name}");
        }
    }
    public void Break()
    {
        if (isBreaking) return;
        isBreaking = true;

        // 1. 즉시 보이지 않게
        gameObject.SetActive(false);

        // 2. 파괴 이펙트 (위치 저장 필요)
        Vector3 position = transform.position;
        if (breakEffectPrefab != null)
        {
            Instantiate(breakEffectPrefab, position, Quaternion.identity);
        }

        // 3. 아이템 드롭
        DropRandomItem();

        // 4. 완전 파괴
        Destroy(gameObject);
    }
    private void DropRandomItem()
    {
    

        if (Random.value > itemDropChance)
        {

            return;
        }

        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            return;
        }

        // 랜덤으로 하나만 선택
        int randomIndex = Random.Range(0, itemPrefabs.Length);
        GameObject selectedItemPrefab = itemPrefabs[randomIndex];


        if (selectedItemPrefab != null)
        {
            Vector3 dropPosition = transform.position;
            GameObject droppedItem = Instantiate(selectedItemPrefab, dropPosition, Quaternion.identity);

        }
    }
    private void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, boxCollider.radius);
        }
    }
}
