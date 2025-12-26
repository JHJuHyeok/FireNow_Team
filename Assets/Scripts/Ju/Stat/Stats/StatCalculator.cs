using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 게임 시작 시 호출
public class StatCalculator
{
    private BattleStat battleStat = new();

    /// <summary>
    /// 게임 시작 또는 장비 변경 시 스탯 변경값 계산
    /// </summary>
    /// <param name="baseStat"> 진화에 영향을 받는 기본 스탯 </param>
    /// <param name="equipStats"> 각 장비 장착 시 변경된 스탯 리스트 </param>
    /// <returns></returns>
    public BattleStat Calculate(BaseStatSO baseStat, List<IStatProvider> equipStats)
    {
        // 계산 시작 전 초기화
        battleStat.ClearRuntimeStats();
        // 기본 스탯값 반영
        baseStat.Apply(battleStat);
        // 각 장비의 스탯 상승값 반영
        foreach (var equip in equipStats)
            equip.Apply(battleStat);

        return battleStat;
    }
}
