using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//<EquipInfo(보유 정보) + EquipDataRunTime(장비 정의)>를
//장착 시스템,UI에 쓰이는 (Equip_ItemBase)로 보내줄 다리 역할
//실제 데이터를 생성하는게 아닌, 데이터 구조와 장비시스템간의 연결시스템
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

    //여기 부분 나중에 레벨업 관련해서 추가할 데이터 필요- 수정 필요
    //아이템 부위별 기본 능력치 이기 때문에, 체력/공격력 공용으로 쓸 생각도 해야돼 
    public override int AttackPower { get { EquipGrade equipGrade = CurrentGradeData;  return equipGrade.startValue; } } 

    //현재 등급에 해당하는 Equipgrade를 리스트에서 찾아 반환 필요
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
