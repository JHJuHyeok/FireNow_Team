using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 각 장비 부위별 스탯 관리 SO
[CreateAssetMenu(menuName = "Stats/EquipStat")]
public class EquipStatSO : ScriptableObject, IStatProvider
{
    public float maxHP;
    public float increaseHpPercent;
    public float attack;
    public float increaseAttackPercent;

    /// <summary>
    /// 장비 장착으로 변경되는 스탯값을 전투 시 스탯의 추가값에 반영
    /// </summary>
    /// <param name="stat"> 전투 시 사용될 스탯 </param>
    public void Apply(BattleStat stat)
    {
        stat.maxHP.additive += maxHP;
        stat.maxHP.multiplier += increaseHpPercent;
        stat.attack.additive += attack;
        stat.attack.multiplier += increaseAttackPercent;
    }

    /// <summary>
    /// 장착 해제 시 모든 값 초기화
    /// </summary>
    public void ClearEquip()
    {
        maxHP = 0f;
        increaseHpPercent = 0f;
        attack = 0f;
        increaseAttackPercent = 0f;
    }
}
