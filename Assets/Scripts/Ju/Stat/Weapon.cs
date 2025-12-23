using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 무기 획득 시 생성할 인스턴스
public class Weapon
{
    public string weaponID { get; private set; }
    public int level { get; private set; }
    public WeaponRuntimeStat runtimeStat { get; private set; }
    public WeaponProvider provider { get; private set; }

    private AbilityDataRuntime data;

    /// <summary>
    /// 무기 생성자
    /// </summary>
    /// <param name="data"> 관련 어빌리티 런타임 데이터 </param>
    /// <param name="level"> 획득 시 레벨 </param>
    public Weapon(AbilityDataRuntime data, int level = 1)
    {
        this.data = data;
        this.weaponID = data.id;
        this.level = level;

        runtimeStat = new WeaponRuntimeStat();

        provider = new WeaponProvider(data, level);
    }

    /// <summary>
    /// 무기 레벨업
    /// </summary>
    public void levelUp()
    {
        level++;
        provider.SetLevel(level);
    }

    /// <summary>
    /// 능력 보유 현황 변화 시 무기 스탯 재계산
    /// </summary>
    /// <param name="battleStat"> 전투 시 스탯 </param>
    public void RecalculateStat(BattleStat battleStat)
    {
        runtimeStat.ClearRuntime();

        provider.Apply(runtimeStat.weaponStat, battleStat);

        runtimeStat.Refresh();
    }
}
