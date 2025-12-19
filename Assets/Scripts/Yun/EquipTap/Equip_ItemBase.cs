using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//모든 아이템(SO 또는 데이터 클래스)를 공통 처리 할 수 있는 추상 클래스
//UI와 장착시스템에서 이 타입 참조(다리역할)
//현재 주혁님이 만들어두신 툴에 없는 정보는 따로 요청할것
public abstract class Equip_ItemBase
{
    //===== 기본 정보 =====
    public abstract string ItemID { get; }
    public abstract string ItemName { get; }
    public abstract string Description { get; }
    public abstract Sprite ItemIcon { get; }

    //===== 장착 정보 =====
    //JSON EquipPart 그대로 사용
    public abstract EquipPart EquipPart { get; }
    public abstract bool CanEquip { get; }

    //===== 성장 정보 =====
    //JSON Grade 그대로 사용 
    public abstract Grade Grade { get; }
    public abstract int Level { get; }
    public abstract int MaxLevel { get; }

    //===== 능력치 ===== 장비부위별 공격력 체력 나누긴 해야함
    public abstract int AttackPower { get; }

    //등급별 설명-런타임 기반 정보 읽어오기-단일값이 아니라서..테이블자체 들고오기
    public abstract EquipDataRuntime SourceEquipData { get; }


    //아래는 JSON EquipData 툴에서 추가 필요한 정보
    //public abstract Sprite NeedPartIcon { get; } //부위별 필요 재료아이콘
    //public abstract int NeedPartCount { get; } //레벨별 필요 재료 갯수
    //public abstract int HavePartCount { get; } //보유 재료 갯수 -이건 플레이어 데이터 쪽
    //public abstract int NeedCoin { get; } //레벨별 필요 코인
    //public abstract int HaveCoin { get; } //보유 코인 - 이건 플레이어 데이터쪽
}
