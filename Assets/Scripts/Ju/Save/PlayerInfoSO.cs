using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 런타임 중 활용 데이터
[CreateAssetMenu(menuName = "Player/PlayerInfo")]
public class PlayerInfoSO : ScriptableObject
{
    public int gold;                // 소지 골드
    public int gem;                 // 소지 보석
    public int stamina;             // 현재 스태미나
    public int maxStamina;          // 최대 스태미나
    public int accountLevel;        // 계정 레벨

    public long lastStaminaTime;    // 종료 시 시각

    public List<EquipInfo> equips = new List<EquipInfo>();      // 장비 목록
    public List<StuffStack> stuffs = new List<StuffStack>();     // 소지품 목록

    public string lastStageId;              // 마지막으로 해금된 스테이지 ID
    public string lastEvolveId;             // 마지막으로 해금한 진화 ID
    public string lastSpecialEvolveId;      // 마지막으로 해금한 특수진화 ID
    public int evolveUnlockSlotCount;       // 진화탭에서 실제로 해금된 슬롯 수 ->임시-윤성원
}

/// <summary>
/// 보유 소지품 및 갯수 확인용
/// </summary>
[System.Serializable]
public class StuffStack
{
    public StuffDataRuntime stuff;
    public int amount;
}

[System.Serializable]
public class EvolveActive
{

    public bool isActive;
}
/// <summary>
/// 스테이지 클리어 여부 확인
/// </summary>
[System.Serializable]
public class StageClear
{
    public StageDataRuntime stage;
    public bool isClear;
}
/// <summary>
/// 보유 장비 등급, 레벨 확인용
/// </summary>
[System.Serializable]
public class EquipInfo
{
    public EquipDataRuntime equip;
    public Grade grade;
    public int level;
    //장착된 상태인지 저장용
    public bool isEquipped;
}
