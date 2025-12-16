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

    public List<EquipData> equips = new List<EquipData>();      // 장비 목록
    public List<StuffStack> stuffs = new List<StuffStack>();     // 소지품 목록
    //public List<EvolveActive> evolves = new List<EvolveData>(); // 해금된 진화 목록
    public List<StageClear> stages = new List<StageClear>();    // 클리어한 스테이지 목록
    //public List<EvolveData> evolves = new List<EvolveData>(); // 해금된 진화 목록
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
