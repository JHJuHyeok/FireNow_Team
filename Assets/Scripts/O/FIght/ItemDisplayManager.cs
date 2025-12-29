using System.Collections.Generic;
using UnityEngine;

public class ItemDisplayManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform itemContainer; // ObjectPanel (아이템들이 배치될 부모)
    public GameObject itemPrefab; // ItemGray 프리팹

    [Header("Layout Settings")]
    public float itemSpacing = 120f; // 아이템 간격 (픽셀)
    public float startX = 0f; // 시작 X 위치


    [Header("Count Range")]
    public int minCount = 1;
    public int maxCount = 10;

    [Header("Item Sprites Pool")]
    public List<Sprite> availableSprites = new List<Sprite>(); // 6개의 스프라이트

    private List<GameObject> spawnedItems = new List<GameObject>();

    private void Start()
    {
        // 게임 시작 시 랜덤 아이템 표시
        DisplayRandomItems();
    }

    /// <summary>
    /// 랜덤으로 3~4개의 아이템 표시
    /// </summary>
    public void DisplayRandomItems()
    {
        // 기존 아이템 제거
        ClearAllItems();

        if (availableSprites.Count == 0)
        {
            Debug.LogError("사용 가능한 스프라이트가 없습니다!");
            return;
        }

        // 3~4개 중 랜덤 선택
        int itemCount = Random.Range(3, 5); // 3 또는 4

        // 스프라이트 리스트 복사 (원본 유지)
        List<Sprite> spritePool = new List<Sprite>(availableSprites);

        // 랜덤으로 선택
        for (int i = 0; i < itemCount && spritePool.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, spritePool.Count);
            Sprite selectedSprite = spritePool[randomIndex];
            
            // 중복 방지 (같은 스프라이트 다시 안 나오게)
            spritePool.RemoveAt(randomIndex);

            // 랜덤 개수 생성
            int randomCount = Random.Range(minCount, maxCount + 1);

            AddItem(spritePool[i], "", randomCount);
        }
    }

    /// <summary>
    /// 아이템 추가
    /// </summary>
    public void AddItem(Sprite itemSprite, string itemName = "", int count = 0)
    {
        if (itemContainer == null || itemPrefab == null)
        {
            //Debug.LogError("ItemContainer 또는 ItemPrefab이 할당되지 않았습니다!");
            return;
        }

        // 아이템 생성
        GameObject newItem = Instantiate(itemPrefab, itemContainer);
        
        // 위치 설정 (가로로 배치)
        RectTransform rectTransform = newItem.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            float xPosition = startX + (spawnedItems.Count * itemSpacing);
            rectTransform.anchoredPosition = new Vector2(xPosition, 0);
        }

        // 아이템 설정
        ItemDisplay itemDisplay = newItem.GetComponent<ItemDisplay>();
        if (itemDisplay != null)
        {
            itemDisplay.Setup(itemSprite, itemName, count);
        }

        spawnedItems.Add(newItem);
    }

    /// <summary>
    /// 모든 아이템 제거
    /// </summary>
    public void ClearAllItems()
    {
        foreach (GameObject item in spawnedItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        spawnedItems.Clear();
    }

    /// <summary>
    /// 특정 인덱스의 아이템 제거
    /// </summary>
    public void RemoveItem(int index)
    {
        if (index < 0 || index >= spawnedItems.Count) return;

        GameObject itemToRemove = spawnedItems[index];
        spawnedItems.RemoveAt(index);
        Destroy(itemToRemove);

        // 나머지 아이템 위치 재정렬
        RefreshLayout();
    }

    /// <summary>
    /// 레이아웃 재정렬
    /// </summary>
    private void RefreshLayout()
    {
        for (int i = 0; i < spawnedItems.Count; i++)
        {
            RectTransform rectTransform = spawnedItems[i].GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float xPosition = startX + (i * itemSpacing);
                rectTransform.anchoredPosition = new Vector2(xPosition, 0);
            }
        }
    }

    /// <summary>
    /// 현재 표시된 아이템 개수
    /// </summary>
    public int GetItemCount()
    {
        return spawnedItems.Count;
    }

    /// <summary>
    /// 특정 개수만큼 랜덤 아이템 표시
    /// </summary>
    public void DisplayRandomItems(int count)
    {
        ClearAllItems();

        if (availableSprites.Count == 0)
        {
            Debug.LogError("사용 가능한 스프라이트가 없습니다!");
            return;
        }

        count = Mathf.Min(count, availableSprites.Count);
        List<Sprite> spritePool = new List<Sprite>(availableSprites);

        for (int i = 0; i < count && spritePool.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, spritePool.Count);
            Sprite selectedSprite = spritePool[randomIndex];
            spritePool.RemoveAt(randomIndex);
            AddItem(selectedSprite);
        }
    }
}