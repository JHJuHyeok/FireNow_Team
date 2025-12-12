using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


//테스트용 아이템 데이터 클래스
//JSON 직렬화 염두, 나중에 JSON<->SO 변환 가능하게
//Equip_ItemBase 상속해서 장비 시스템이랑 호환되게

[System.Serializable]
public class ItemData_Test : Equip_ItemBase
{
    [Header("기본 정보")]
    public int itemID;
    public string itemName;
    public ItemGrade grade;

    [Header("아이템 설명")]
    public string description;

    [Header("아이템 능력치")]
    public int attackPower;
    public int itemLevel;

    [Header("아이템 아이콘")]
    public Sprite itemIcon;

    [Header("레벨업 관련 재료 정보")]
    public Sprite needPartIcon;
    public int needPartCount;
    public int havePartCount;

    [Header("레벨업 관련 코인 정보")]
    public int needCoin;
    public int haveCoin;

    [Header("장착가능 여부")]
    public bool canEquip;
    public EquipSlotType equipSlotType;

    //이제 여기서 오버라이드 (공통 인터페이스 연결부분)
    public override int ItemID { get { return itemID; } }
    public override string ItemName { get { return itemName; } }
    public override ItemGrade Grade { get { return grade; } }
    public override string Description { get { return description; } }
    public override int AttackPower { get { return attackPower; } }
    public override int ItemLevel { get { return itemLevel; } }
    public override Sprite ItemIcon { get { return itemIcon; } }
    public override Sprite NeedPartIcon { get { return needPartIcon; } }
    public override int NeedPartCount { get { return needPartCount; } }
    public override int HavePartCount { get { return havePartCount; } }
    public override int NeedCoin { get { return needCoin; } }
    public override int HaveCoin { get { return haveCoin; } }
    public override bool CanEquip { get { return canEquip; } }
    public override EquipSlotType EquipSlotType { get { return equipSlotType; } }
}
