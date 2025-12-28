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
/// ++세이브 로드 과정중 장착 상태를 저장할 로직이 없어서 추가
/// 기존 인벤토리에서 Add/remove 하던 과정 삭제
/// EquipInfo쪽 장착플래그만 변경해서
/// 인벤토리의 아이템은 isEquiped(false)인 장비만 보여주고,
/// 장착칸의 아이템은 isEquiped(true)인 장비를 파트별로 찾아서 표시
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
    private void OnEnable()
    {
        StartCoroutine(EquipTapEntryCO());
    }

    //장착칸,인벤토리 둘다 갱신 함수
    public void RefreshAll()
    {
        RefreshEquipSlotUI();
        RefreshInventoryUI();
    }

    /// <summary>
    /// 인벤토리 UI갱신
    /// 전체 보유목록중 isEquiped(false)만 표시
    /// </summary>
    public void RefreshInventoryUI()
    {
        //리스트 안 목록 일단 비워주고,
        _inventoryItems.Clear();

        //진짜 인벤토리 목록을 순회하면서
        for (int i = 0; i < playerInfoSO.equips.Count; i++)
        {
            //보유 정보 장비정의정보를 각 요소에 넣어주고,
            EquipInfo equipInfo = playerInfoSO.equips[i];
            if (equipInfo == null) continue;
            if (equipInfo.equip == null) continue;
            //장착된 장비는 인벤토리에서 숨기기
            if (equipInfo.isEquipped == true) continue;
            //Equip_ItemBase가 받을수 있게 브릿지 거쳐주고
            EquipInfoBridge bridge = new EquipInfoBridge(equipInfo);
            //보여주기용 리스트에 추가
            _inventoryItems.Add(bridge);
        }
        inventoryUI.Refresh(_inventoryItems);
    }
    /// <summary>
    /// 장착칸 UI갱신
    /// -슬롯 모두 비우고,
    /// -파트별로 isEquiped(true)인 장비를 찾아서 다시 채우기
    /// </summary>
    public void RefreshEquipSlotUI()
    {
        //슬롯 UI 전부 초기화
        for (int i = 0; i < equipSlots.Count; i++)
        {
            EquipSlot slot = equipSlots[i];
            if (slot == null) continue;
            Equip_ItemBase current = slot.GetItem();
            if (current != null)
            {
                slot.Clear(current);
            }
        }
        //각 슬롯 파트별로 장착된 장비찾아서 세팅
        for (int i = 0; i < equipSlots.Count; i++)
        {
            EquipSlot slot = equipSlots[i];
            if (slot == null) continue;
            //어느부위인지 특정해주고
            EquipPart part = slot.equipPart;
            //장착중인 장비 info 가져와서
            EquipInfo equipedInfo = FindEquipedInfoByPart(part);
            if (equipedInfo == null) continue;
            //Equip_ItemBase가 받을수 있게 브릿지 거쳐주고
            EquipInfoBridge bridge = new EquipInfoBridge(equipedInfo);
            //슬롯 세팅
            slot.Set(bridge);
        }
    }

    /// <summary>
    /// 특정 파트에서 현재 장착중인 EquipInfo 찾아서 반환하는 함수
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    private EquipInfo FindEquipedInfoByPart(EquipPart part)
    {
        for (int i = 0; i < playerInfoSO.equips.Count; i++)
        {
            EquipInfo info = playerInfoSO.equips[i];
            if (info == null) continue;
            if (info.equip == null) continue;
            //해당 부위, 장착중이면 인포 반환
            if (info.equip.part == part && info.isEquipped == true)
            {
                return info;
            }
        }
        return null;
    }

    /// <summary>
    /// 장착 처리 함수
    /// </summary>
    /// <param name="item"></param>
    public void Equip(Equip_ItemBase item)
    {
        //UI에서 넘어온 아이템은 EquipInfoBridge
        EquipInfoBridge bridge = item as EquipInfoBridge;
        if (bridge == null) return;
        //원본 EquipInfo 참조
        EquipInfo targetInfo = bridge.ItemBaseSourceInfo;
        if (targetInfo == null) return;
        if (targetInfo.equip == null) return;

        //부위 특정
        EquipPart part = targetInfo.equip.part;

        //같은 부위에 이미 장착 장비 있으면 해제
        EquipInfo alreadyEquiped = FindEquipedInfoByPart(part);
        //플래그도 해제
        if (alreadyEquiped != null)
        {
            alreadyEquiped.isEquipped = false;
        }
        Debug.Log("장착전, isEquiped 상태="+ targetInfo.isEquipped);
        //선택 장비를 장착처리
        targetInfo.isEquipped = true;
        Debug.Log("장착후, isEquiped 상태=" + targetInfo.isEquipped);
        //장착칸,인벤토리 UI갱신
        RefreshAll();
        //UI 갱신후 스탯 재계산
        EquipStatSystem.Instance.RecalculateFromEquipSlots(equipSlots);
        //저장 지점
        SaveManager.Instance.Save();
    }

    //장착 해제 처리 함수
    public void UnEquip(Equip_ItemBase item)
    {
        //UI에서 넘어온 아이템은 EquipInfoBridge
        EquipInfoBridge bridge = item as EquipInfoBridge;
        if (bridge == null) return;
        //원본 EquipInfo 참조
        EquipInfo targetInfo = bridge.ItemBaseSourceInfo;
        if (targetInfo == null) return;
        //장착해제
        targetInfo.isEquipped = false;
        //UI갱신
        RefreshAll();
        //스탯 재계산
        EquipStatSystem.Instance.RecalculateFromEquipSlots(equipSlots);
        //저장지점
        SaveManager.Instance.Save();
    }
    //ui갱신 문제 임시
    private IEnumerator EquipTapEntryCO()
    {
        //한프레임 대기
        yield return null;
        RefreshAll();
        //스탯 재계산
        EquipStatSystem.Instance.RecalculateFromEquipSlots(equipSlots);
    }
}
