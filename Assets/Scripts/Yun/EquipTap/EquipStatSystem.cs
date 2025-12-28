using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipStatSystem : MonoBehaviour
{
    public static EquipStatSystem Instance { get; private set; }

    //진화 해금 반영된 스탯SO
    [Header("진화 반영된 기본 스탯")]
    [SerializeField] private BaseStatSO baseStatSO;

    //각 장착 슬롯 SO
    [Header("슬롯별 런타임 스탯 컨테이너 6개")]
    [SerializeField] private EquipStatSO weaponSlotStat;
    [SerializeField] private EquipStatSO necklaceSlotStat;
    [SerializeField] private EquipStatSO gloveSlotStat;
    [SerializeField] private EquipStatSO armorSlotStat;
    [SerializeField] private EquipStatSO beltSlotStat;
    [SerializeField] private EquipStatSO shoesSlotStat;

    [Header("장비탭 스탯 표시 UI")]
    [SerializeField] private EquipTapStatUI equipTabUI;

    //스탯 계산기(메인메뉴용)
    private StatCalculator statCalculator;

    //6개 슬롯 SO를 provider리스트로 묶어서 Calculate에 넘길용도
    private List<IStatProvider> slotProviders;

    //최근 계산 결과를 보관할 용도(저장/다른 UI에서 쓰기 가능)
    public BattleStat CurrentBattleStat { get; private set; }

    private void Awake()
    {
        Instance = this;
        //계산기 생성
        statCalculator = new StatCalculator();
        //provider 리스트 생성
        slotProviders = new List<IStatProvider>(6);
        //각 슬롯 SO등록
        slotProviders.Add(weaponSlotStat);          
        slotProviders.Add(necklaceSlotStat);        
        slotProviders.Add(gloveSlotStat);           
        slotProviders.Add(armorSlotStat);           
        slotProviders.Add(beltSlotStat);            
        slotProviders.Add(shoesSlotStat);
        //시작 시 슬롯 컨테이너 초기화
        ClearAllSlotStats();
    }

    /// <summary>
    /// 장착/해제/레벨업/진화해금 등으로 스탯이 바뀐 직후 호출
    /// </summary>
    public void RecalculateFromEquipSlots(List<EquipSlot> equipSlots)
    {
        //슬롯 컨테이너 6개 모두 초기화(이전 장착 영향 제거)
        ClearAllSlotStats();

        //현재 장착된 아이템들 슬롯에서 읽어서, 해당 슬롯 SO에 스탯 값 채우기
        for (int i = 0; i < equipSlots.Count; i++)
        {
            //i번째 슬롯
            EquipSlot slot = equipSlots[i];
            //슬롯 없으면 일단 스킵
            if (slot == null) continue;
            //슬롯에 장착된 아이템
            Equip_ItemBase item = slot.GetItem();
            //장착 아이템 없으면 스킵
            if (item == null) continue;
            //부위에 맞는 슬롯 SO 찾기
            EquipStatSO slotSO = GetSlotStatSO(item.EquipPart);
            //못 찾았으면 일단 스킵
            if (slotSO == null) continue;
            //아이템의 스탯 타입과 값을 슬롯 SO에 반영
            ApplyItemStatToSlotSO(slotSO, item);
        }

        //계산기 인스턴스 새로 만들고(내부 BattleStat 재사용되는 문제때문)
        StatCalculator statCalculator = new StatCalculator();
        //베이스+장비 계산
        BattleStat stat = statCalculator.Calculate(baseStatSO, slotProviders);
        //final스탯 값 내고
        stat.Refresh();

        //결과 캐싱
        CurrentBattleStat = stat;

        //장비탭 UI 갱신(연결되어 있으면)
        equipTabUI.RefreshStatUI(stat);
    }

    /// <summary>
    /// 아이템이 올려주는 스탯을 슬롯 SO에 채워넣는 함수
    /// 장비 3종류당 2타입으로 나뉨 공격력,최대체력 StatType에 정의되어있음
    /// StatType.attack -> slotSO.attack에 넣기
    /// StatType.health -> slotSO.maxHP에 넣기
    /// </summary>
    private void ApplyItemStatToSlotSO(EquipStatSO slotSO, Equip_ItemBase item)
    {
        //현재 레벨/등급 기준 증가량 저장
        int value = item.StatValue;

        if (item.StatType == StatType.attack)
        {
            //공격력 증가량 적용
            slotSO.attack = value;
            //배율관련 능력은 아직 미사용(임시)
            slotSO.increaseAttackPercent = 0.0f;
        }
        else if (item.StatType == StatType.health)
        {
            //체력 증가량 적용
            slotSO.maxHP = value;
            //여기도 배율은 아직 미사용
            slotSO.increaseHpPercent = 0.0f;
        }
    }

    /// <summary>
    /// EquipPart에 맞는 슬롯 SO와 1:1 매핑하고 반환하는 함수
    /// </summary>
    private EquipStatSO GetSlotStatSO(EquipPart part)
    {
        if (part == EquipPart.weapon) return weaponSlotStat;
        if (part == EquipPart.necklace) return necklaceSlotStat;
        if (part == EquipPart.glove) return gloveSlotStat;
        if (part == EquipPart.armor) return armorSlotStat;
        if (part == EquipPart.belt) return beltSlotStat;
        if (part == EquipPart.shoes) return shoesSlotStat;

        return null;
    }

    /// <summary>
    /// 슬롯 컨테이너 6개 전부 0으로 초기화
    /// </summary>
    private void ClearAllSlotStats()
    {
        if (weaponSlotStat != null) weaponSlotStat.ClearEquip();
        if (necklaceSlotStat != null) necklaceSlotStat.ClearEquip();
        if (gloveSlotStat != null) gloveSlotStat.ClearEquip();
        if (armorSlotStat != null) armorSlotStat.ClearEquip();
        if (beltSlotStat != null) beltSlotStat.ClearEquip();
        if (shoesSlotStat != null) shoesSlotStat.ClearEquip();
    }
}
