using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//장비 장착/해체와 이벤토리 이동을 제어하는 컨트롤러
public class EquipControl : MonoBehaviour
{
    public static EquipControl Instance;

    [Header("장비 슬롯 리스트")]
    public List<EquipSlot> equipSlots;

    [Header("플레이어 인벤토리 리스트")]
    public List<Equip_ItemBase> inventoryItems = new List<Equip_ItemBase>();

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 장착 처리 함수
    /// </summary>
    /// <param name="item"></param>
    public void Equip(Equip_ItemBase item)
    {
        for (int i = 0; i < equipSlots.Count; i++)
        {
            //같은 타입의 장비를 장착했을경우
            if (equipSlots[i].equipType == item.EquipSlotType)
            {
                //기존 장비는 인벤토리로 돌려놓고
                Equip_ItemBase preItem = equipSlots[i].GetItem();
                if (preItem != null)
                {
                    inventoryItems.Add(preItem);
                }

                //장비슬롯에 아이템 장착
                equipSlots[i].Set(item);

                //인벤토리에서는 제거
                inventoryItems.Remove(item);

                //인벤토리 UI에서 갱신
                InventoryUI ui = FindObjectOfType<InventoryUI>();
                ui.Refresh(inventoryItems);
                return;
            }
        }
    }

    //장착 해제 처리 함수
    public void UnEquip(Equip_ItemBase item)
    {
        for (int i = 0; i < equipSlots.Count; i++)
        {
            if (equipSlots[i].GetItem() == item)
            {
                //장비 슬롯에선 빼주고
                equipSlots[i].Clear(item);

                //인벤토리아이템 에는 추가
                inventoryItems.Add(item);

                InventoryUI ui = FindAnyObjectByType<InventoryUI>();
                ui.Refresh(inventoryItems);
                return;
            }
        }
    }
}
