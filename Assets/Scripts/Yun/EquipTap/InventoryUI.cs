using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//그리드 레이아웃이 붙은 컨텐트 아래로 쭈르륵 붙어나올 슬롯 프리팹들 염두
//플레이어의 보유 아이템 데이터와 관련.
//장착하면 인벤토리 UI에서 삭제
//1.슬롯 종류는 2가지 인벤토리용, 장착형
//2.변화형 패널 1가지 슬롯 종류에 따라 버튼 한개 변화
//3.인벤토리를 UI를 전체 관리할 cs 필요
//4.아이템 장착/해제 제어할 데이터 컨트롤러 필요

//장비탭 최상위에 컴포넌트로 적용 -그리드 레이아웃 그룹 기반 동적 인벤토리
public class InventoryUI : MonoBehaviour
{
    [Header("슬롯이 보여질 곳(GridLayout오브젝트)")]
    [SerializeField] Transform itemGrid;

    [Header("인벤토리 슬롯 프리팹")]
    [SerializeField] InventorySlot slotPrefab; 

    //현재 아이템그리드에 생성된 슬롯들을 저장
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
