using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 진화에 영향을 받는 기본 스탯
[CreateAssetMenu(menuName = "Stats/BaseStat")]
public class BaseStatSO : ScriptableObject, IStatProvider
{
    public int maxHP;
    public int attack;
    public int defence;
    public int getHPWithMeat;

    /// <summary>
    /// 진화로 영향을 받은 스탯을 전투 시 스탯의 기본값에 반영
    /// </summary>
    /// <param name="stat"> 전투 시 사용될 스탯 </param>
    public void Apply(BattleStat stat)
    {
        stat.maxHP.baseValue += maxHP;
        stat.attack.baseValue += attack;
        stat.defence.baseValue += defence;
        stat.getHPWithMeat.baseValue += getHPWithMeat;
    }
}
