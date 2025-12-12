using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//모든 아이템(SO 또는 데이터 클래스)를 공통 처리 할 수 있는 추상 클래스
//UI와 시스템에서 이 타입 참조(다리역할)
public abstract class Equip_ItemBase
{
    public abstract int ItemID { get; }
    public abstract string ItemName { get; }

    public abstract ItemGrade Grade { get; }

    public abstract string Description { get; }

    public abstract int AttackPower { get; }
    public abstract int ItemLevel { get; }

    public abstract Sprite ItemIcon { get; }

    public abstract Sprite NeedPartIcon { get; }

    public abstract int NeedPartCount { get; }
    public abstract int HavePartCount { get; }

    public abstract int NeedCoin { get; }
    public abstract int HaveCoin { get; }

    public abstract bool CanEquip { get; }

    public abstract EquipSlotType EquipSlotType { get; }
}
