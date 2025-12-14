using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이후 팩토리에서 아이템 생성해서 가져올건데(상점 가챠라던지), 일단 뭐 없으니까
//아이템 생성하고 시작하는걸로 테스트
public class StartInventory_TEST : MonoBehaviour
{
    public InventoryUI inventoryUI;
    public EquipControl equipControl;

    private void Start()
    {
        PlayerEquipItem weapon1 = EquipItem_Factory.Create("Weapon_01",1);

        equipControl.inventoryItems.Add(weapon1);
        inventoryUI.Refresh(equipControl.inventoryItems);
    }
}
