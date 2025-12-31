using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 장비탭 최상위에 컴포넌트로 적용 -그리드 레이아웃 그룹 기반 동적 인벤토리
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("슬롯이 보여질 곳(GridLayout오브젝트)")]
    [SerializeField] Transform itemGrid;

    [Header("인벤토리 슬롯 프리팹")]
    [SerializeField] InventorySlot slotPrefab; 

    //현재 아이템그리드에 생성된 슬롯들 리스트로 저장
    private readonly List<InventorySlot> slotList = new List<InventorySlot>();

    /// <summary>
    /// 인벤토리 UI를 갱신해줄 함수
    /// </summary>
    /// <param name="items"></param>
    public void Refresh(List<Equip_ItemBase> items)
    {
        //기존 슬롯은 삭제하고,
        for (int i = 0; i < slotList.Count; i++)
        {
            Destroy(slotList[i].gameObject);
        }
        slotList.Clear();

        //변경된 내용에 따라 슬롯 추가
        for (int i = 0; i < items.Count; i++)
        {
            InventorySlot newSlot = Instantiate(slotPrefab, itemGrid);
            newSlot.Set(items[i]);
            slotList.Add(newSlot);
        }
    }
}
