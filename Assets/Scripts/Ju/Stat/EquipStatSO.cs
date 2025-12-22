using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
