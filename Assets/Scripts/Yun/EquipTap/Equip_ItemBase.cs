using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 아이템 공통 처리 할 추상 클래스
/// EquipInfoBridge와 연계
/// </summary>
public abstract class Equip_ItemBase
{
    //===== 기본 정보 =====
    public abstract string ItemID { get; }
    public abstract string ItemName { get; }
    public abstract string Description { get; }
    public abstract Sprite ItemIcon { get; }

    //===== 장착 정보 =====
    public abstract EquipPart EquipPart { get; }
    public abstract bool CanEquip { get; }

    //===== 성장 정보 ===== 
    public abstract Grade Grade { get; }
    public abstract int Level { get; }
    public abstract int MaxLevel { get; }

    //===== 능력치 =====
    public abstract StatType StatType { get; }
    public abstract int StatValue { get; }

    //===== 등급별 설명 =====
    public abstract EquipDataRuntime SourceEquipData { get; }

}
