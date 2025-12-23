using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProvider : IWeaponStatProvider
{
    private AbilityDataRuntime data;
    private int level;

    public WeaponProvider(AbilityDataRuntime data, int level = 1)
    {
        this.data = data;
        this.level = level;
    }

    public void Apply(WeaponStat weaponStat, BattleStat battleStat)
    {
        AbilityLevelData levelData = data.levels[level - 1];
        
        weaponStat.damage.multiplier += levelData.damage;                   // 데미지 비율 상승
        weaponStat.cooldown.additive -= levelData.cooldown;                 // 쿨다운 
        weaponStat.projectileCount.additive += levelData.projectileCount;   // 투사체 수
        weaponStat.range.additive += levelData.range;                       // 공격 범위
        weaponStat.speed.additive += levelData.speed;                       // 투사체 속도
        weaponStat.duration.additive += levelData.duration;                 // 효과 지속 시간

        weaponStat.damage.additive += battleStat.attack.calculateValue;                 // 데미지에 플레이어 스탯 적용
        weaponStat.cooldown.multiplier -= battleStat.cooldown.calculateValue - 1f;      // 쿨다운에 플레이어 스탯 적용
        weaponStat.range.multiplier += battleStat.range.calculateValue - 1f;            // 공격 범위에 플레이어 스탯 적용
        weaponStat.speed.multiplier += battleStat.projectileSpeed.calculateValue - 1f;  // 투사체 속도에 플레이어 스탯 적용
        weaponStat.duration.multiplier += battleStat.duration.calculateValue - 1f;      // 효과 지속 시간에 플레이어 스탯 적용
    }
}
