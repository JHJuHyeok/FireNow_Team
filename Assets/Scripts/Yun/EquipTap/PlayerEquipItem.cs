using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//플레이어가 실제로 보유하는 장비 관리->런타임 기반으로 장비관리
public class PlayerEquipItem : Equip_ItemBase
{
    private EquipData equipData;           //JSON 원본 데이터
    private EquipGrade currentGradeData;   //현재 등급 데이터
    private int level;                     //레벨
    private Sprite icon;                   //로드된 아이콘

    public PlayerEquipItem(EquipData data,EquipGrade gradeData,int startLevel,Sprite loadedIcon)
    {
        equipData = data;
        currentGradeData = gradeData;
        level = startLevel;
        icon = loadedIcon;
    }

    //=====인터페이스 구현======
    public override string ItemID { get { return equipData.id; } }
    public override string ItemName { get { return equipData.equipName; } }
    public override string Description { get { return equipData.descript; } }
    public override Sprite ItemIcon { get { return icon; } }

    public override EquipPart EquipPart { get { return equipData.part; } }

    public override bool CanEquip { get { return true; } }

    public override Grade Grade { get { return currentGradeData.grade; } }
    public override int Level { get { return level; } }
    public override int MaxLevel { get { return currentGradeData.maxLevel; } }

    public override int AttackPower{ get { return currentGradeData.startValue + (level - 1); } }
}
