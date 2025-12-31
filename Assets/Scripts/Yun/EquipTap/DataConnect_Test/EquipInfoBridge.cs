using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <EquipInfo(보유 정보) + EquipDataRunTime(장비 정의)>를
/// 장착 시스템,UI에 쓰이는 (Equip_ItemBase)로 보내줄 다리 역할
/// 실제 데이터를 생성하는게 아닌, 데이터 구조와 장비시스템간의 연결시스템
/// </summary>
public class EquipInfoBridge : Equip_ItemBase
{
    //PlayerInfoSO.equips 에 들어있는 EquipInfo의 원본을 참조
    private readonly EquipInfo _equipInfo;

    //Equip_ItemBase에서 원본 EquipInfo로 돌아갈 출구역할
    public EquipInfo ItemBaseSourceInfo
    {
        get { return _equipInfo; }
    }

    //런타임 데이터용
    public EquipInfoBridge(EquipInfo equipInfo)
    {
        _equipInfo = equipInfo;
    }

    //=====인터페이스 구현======
    public override string ItemID { get { return _equipInfo.equip.id; } }
    public override string ItemName { get { return _equipInfo.equip.equipName; } }
    public override string Description { get { return _equipInfo.equip.descript; } }
    public override Sprite ItemIcon { get { return _equipInfo.equip.icon; } }
    public override EquipPart EquipPart { get { return _equipInfo.equip.part; } }
    public override bool CanEquip { get { return true; } }
    public override Grade Grade { get { return _equipInfo.grade; } }
    public override int Level { get { return _equipInfo.level; } }
    public override int MaxLevel { get { EquipGrade equipGrade = CurrentGradeData; return equipGrade.maxLevel; } }

    //등급별 설명 텍스트를 데이터 기반으로 읽기위한 원본 런타임 데이터
    public override EquipDataRuntime SourceEquipData { get { return _equipInfo.equip; } }

    //장비 레벨업시 능력치 증가량은 고정수치로 구현
    private const int levelUpIncrease = 4;
    public override StatType StatType { get { return _equipInfo.equip.stat; } }
    
    public override int StatValue
    { get { EquipGrade equipGrade = CurrentGradeData;
            if (equipGrade == null) return 0;
            //레벨이 1보다 작으면 무조건 1로(최소 레벨 보장 안전장치)
            int level = Mathf.Max(1, _equipInfo.level);
            //시작 기본능력치 + 레벨 증가량 더해주기
            return equipGrade.startValue + (level - 1) * levelUpIncrease; } 
    } 

    /// <summary>
    /// 현재 등급에 해당하는 Equipgrade를 리스트에서 찾아 반환
    /// </summary>
    private EquipGrade CurrentGradeData
    {
        get
        {
            List<EquipGrade> grades = _equipInfo.equip.equipGrades;

            for (int i = 0; i < grades.Count; i++)
            {
                if (grades[i].grade == _equipInfo.grade)
                    return grades[i];
            }
            return null;
        }
    }
}
