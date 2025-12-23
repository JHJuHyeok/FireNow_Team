using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatManager
{
    public BattleStat battleStat { get; private set; }

    /// <summary>
    /// 플레이어 스탯 매니저 생성자
    /// </summary>
    public PlayerStatManager()
    {
        battleStat = new BattleStat();
    }

    /// <summary>
    /// 모든 무기 스탯 재계산
    /// </summary>
    /// <param name="weapons"></param>
    public void RecalculateAllStats(WeaponsManager weaponManager, PassiveManager passiveManager)
    {
        // 플레이어 스탯에 패시브 적용
        passiveManager.ApplyAll(battleStat);
        // 무기 스탯에 정보 반영
        weaponManager.RecalculateAll(battleStat);
    }
}
