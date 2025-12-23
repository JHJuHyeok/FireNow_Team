using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 각 무기에 대한 정보 제공자
public class WeaponProvider : IWeaponStatProvider
{
    private AbilityDataRuntime data;        // stat, 레벨 데이터 확보
    private int level;                      // 레벨

    /// <summary>
    /// 인스턴스 생성 시 어빌리티 런타임 데이터와 레벨 정보 입력
    /// </summary>
    /// <param name="data"> 가져오려는 어빌리티 런타임 데이터 </param>
    /// <param name="level"> 무기 레벨 </param>
    public WeaponProvider(AbilityDataRuntime data, int level)
    {
        this.data = data;
        this.level = level;
    }

    public void SetLevel(int level)
    {
        this.level = level;
    }

    /// <summary>
    /// 무기 스탯에 전투 시 스탯을 적용하는 함수
    /// </summary>
    /// <param name="weaponStat"> 적용 대상이 되는 무기 스탯 </param>
    /// <param name="battleStat"> </param>
    public void Apply(WeaponStat weaponStat, BattleStat battleStat)
    {
        AbilityLevelData levelData = data.levels[level - 1];
        
        weaponStat.damage.multiplier += levelData.damageRate;               // 데미지 비율 상승
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
