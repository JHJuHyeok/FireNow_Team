using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager
{
    private WeaponsManager weaponManager;
    private PassiveManager passiveManager;

    /// <summary>
    /// 진화 매니저에 무기와 패시브 매니저 할당
    /// </summary>
    /// <param name="weaponManager"> 사용할 무기 매니저 </param>
    /// <param name="passiveManager"> 사용할 패시브 매니저 </param>
    public EvolutionManager(WeaponsManager weaponManager, PassiveManager passiveManager)
    {
        this.weaponManager = weaponManager;
        this.passiveManager = passiveManager;
    }

    /// <summary>
    /// 지니고 있는 모든 무기의 진화 시도
    /// </summary>
    public void TryEvolveAll()
    {
        foreach (var weapon in weaponManager.weaponDict)
        {
            TryEvolveWeapon(weapon.Value);
        }
    }

    /// <summary>
    /// 무기 진화 시도
    /// </summary>
    /// <param name="weapon"> 진화를 시도하는 무기 </param>
    private void TryEvolveWeapon(Weapon weapon)
    {
        var evolution = weapon.data.evolution;

        // 진화 조건이 없으면 return
        if (evolution == null) return;

        // 무기가 최대 레벨이 아니면 return
        if (weapon.level != weapon.data.maxLevel)
            return;

        // 요구 패시브가 없으면 return
        if (!passiveManager.HasPassive(evolution.requireItem))
            return;

        ExcuteEvolve(weapon);
    }

    /// <summary>
    /// 기존 무기를 weaponManager에서 제거하고 진화 무기로 대체
    /// </summary>
    /// <param name="evolvingWeapon"> 진화 대상인 무기 </param>
    private void ExcuteEvolve(Weapon evolvingWeapon)
    {
        string resultWeaponId = evolvingWeapon.data.evolution.result;

        // 데이터베이스에서 진화 무기 호출
        AbilityData evolveData = AbilityDatabase.GetAbility(resultWeaponId);
        AbilityDataRuntime runtimeData = new AbilityDataRuntime(evolveData);

        // 기존 무기 제거
        weaponManager.RemoveWeapon(evolvingWeapon.data.id);
        // 진화 무기 삽입
        weaponManager.AddWeapon(runtimeData);
    }
}
