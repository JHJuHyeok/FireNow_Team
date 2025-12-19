using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 장비 장착/해제 처리 컨트롤러
/// 실제 데이터(PlayerInfoSO.equips)를 기준으로 인벤토리 UI 갱신
/// 
/// [반드시 지켜야 할 원칙]
/// 1.진짜 데이터는 PlayerInfoSo.equips (List<EquipInfo>)
/// 2.UI에 보여질 데이터는 EquipInfoBridge를 거친 Equip_ItemBase
/// 3.UI용 리스트(_inventoryItems)는 직접 Add/Remove 하지 않음-진짜 중요 진짜
///  ->항상 RefreshInventoryUI()에서만 리스트 요소 추가
/// </summary>
public class EquipControl : MonoBehaviour
{
    public static EquipControl Instance;

    //인벤토리 목록의 진짜 소스는 PlayerInfoSO
    [Header("런타임 단일 소스 = PlayerInfoSO")]
    [SerializeField] private PlayerInfoSO playerInfoSO;

    [Header("장비 슬롯 리스트")]
    public List<EquipSlot> equipSlots;

    [Header("인벤토리 UI")]
    [SerializeField] InventoryUI inventoryUI;

    /// <summary>
    /// 실제 데이터를 건드리는게 아니라, UI보여주기용 임시리스트
    /// 절대 Equip,UnEquip에서 직접 수정하면 안됨 ->브릿지 꼭 거칠것
    /// EquipInfo->EquipInfoBridge->Equip_ItemBase 순서 잊지말것
    /// </summary>
    private readonly List<Equip_ItemBase> _inventoryItems = new List<Equip_ItemBase>();

    private void Awake()
    {
        Instance = this;
    }

    public void RefreshInventoryUI()
    {
        //리스트 안 목록 일단 비워주고,
        _inventoryItems.Clear();

        //진짜 인벤토리 목록을 순회하면서
        for (int i = 0; i < playerInfoSO.equips.Count; i++)
        {
            //보유 정보 장비정의정보를 각 요소에 넣어주고,
            EquipInfo equipInfo = playerInfoSO.equips[i];
            //Equip_ItemBase가 받을수 있게 브릿지 거쳐주고
            EquipInfoBridge bridge = new EquipInfoBridge(equipInfo);
            //보여주기용 임시리스트에 추가
            _inventoryItems.Add(bridge);
        }
        inventoryUI.Refresh(_inventoryItems);
    }

    /// <summary>
    /// 장착 처리 함수
    /// </summary>
    /// <param name="item"></param>
    public void Equip(Equip_ItemBase item)
    {
        //UI에서 넘어온 아이템은 EquipInfoBridge
        EquipInfoBridge bridge = item as EquipInfoBridge;

        for (int i = 0; i < equipSlots.Count; i++)
        {
            //같은 타입의 장비를 장착했을경우
            if (equipSlots[i].equipPart == item.EquipPart)
            {
                //기존 장비는 인벤토리로 돌려놓고
                Equip_ItemBase preItem = equipSlots[i].GetItem();
                if (preItem != null)
                {
                    EquipInfoBridge preBridge = preItem as EquipInfoBridge;
                    //이전 장착 장비를 실제 이전 데이터로 되돌림
                    playerInfoSO.equips.Add(preBridge.ItemBaseSourceInfo);
                    
                    //++여기서 이전 장착 능력치 해제
                    PlayerEquip_Stat.Instance.RemoveEquipStat(preItem);
                }

                //장비슬롯에 아이템 장착
                equipSlots[i].Set(item);

                //실제 데이터에서 제거
                playerInfoSO.equips.Remove(bridge.ItemBaseSourceInfo);

                //++여기서 장착 능력치 추가
                PlayerEquip_Stat.Instance.AddEquipStat(item);

                //인벤토리 브릿지 UI 갱신
                RefreshInventoryUI();
                return;
            }
        }
    }

    //장착 해제 처리 함수
    public void UnEquip(Equip_ItemBase item)
    {
        //UI에서 넘어온 아이템은 EquipInfoBridge
        EquipInfoBridge bridge = item as EquipInfoBridge;

        for (int i = 0; i < equipSlots.Count; i++)
        {
            if (equipSlots[i].GetItem() == item)
            {
                //장비 슬롯에선 빼주고
                equipSlots[i].Clear(item);

                //인벤토리 아이템에는 추가 - 실제 데이터에 추가
                playerInfoSO.equips.Add(bridge.ItemBaseSourceInfo);

                //++여기서 장착 능력치 해제
                PlayerEquip_Stat.Instance.RemoveEquipStat(item);

                //UI갱신
                RefreshInventoryUI();
                return;
            }
        }
    }
}
