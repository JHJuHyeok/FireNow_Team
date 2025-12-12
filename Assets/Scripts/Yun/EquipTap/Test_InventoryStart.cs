using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//테스트 데이터 인벤토리 넣고 UI 갱신 되는거 확인용
public class Test_InventoryStart : MonoBehaviour
{
    public Test_ItemGenerator generator;
    public InventoryUI inventoryUI;
    public EquipControl equipControl;

    private void Start()
    {
        //테스트 아이템을 인벤토리아이템 에 대입
        equipControl.inventoryItems = generator.items;

        //인벤토리 슬롯 생성
        inventoryUI.Refresh(equipControl.inventoryItems);
    }
}
