using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 런타임 도중 무기가 사용하게 될 스탯
public class WeaponRuntimeStat
{
    public WeaponStat weaponStat = new();

    /// <summary>
    /// 스탯 초기화
    /// </summary>
    public void ClearRuntime()
    {
        weaponStat.ClearWeaponStats();
    }

    /// <summary>
    /// 최종 스탯 재계산
    /// </summary>
    public void Refresh()
    {
        weaponStat.CalculateFinalValues();
    }
}
