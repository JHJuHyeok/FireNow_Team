using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/PlayerInfo")]
public class PlayerInfoSO : ScriptableObject
{
    public int gold;                // 소지 골드
    public int gem;                 // 소지 보석
    public int stamina;             // 현재 스태미나
    public int maxStamina;          // 최대 스태미나

    public List<EquipData> equips = new List<EquipData>();      // 장비 목록
    public List<StuffStack> stuffs = new List<StuffStack>();     // 소지품 목록
    //public List<EvolveData> evolves = new List<EvolveData>(); // 해금된 진화 목록
}

[System.Serializable]
public class StuffStack
{
    public StuffData stuff;
    public int amount;
}
