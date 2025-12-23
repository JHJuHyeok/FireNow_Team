using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 패시브 능력 관리
public class PassiveProvider : IStatProvider
{
    private AbilityDataRuntime data;
    private int level;

    /// <summary>
    /// 인스턴스 생성 시 어빌리티 런타임 데이터와 레벨 정보 입력
    /// </summary>
    /// <param name="data"> 가져오려는 어빌리티 런타임 데이터 </param>
    /// <param name="level"> 패시브 고유 레벨(입력 안하면 1) </param>
    public PassiveProvider(AbilityDataRuntime data, int level = 1)
    {
        this.data = data;
        this.level = level;
    }

    /// <summary>
    /// 패시브에 의한 능력치 변화를 플레이어 스탯에 반영
    /// </summary>
    /// <param name="stat"> 플레이어 전투 시 스탯 </param>
    public void Apply(BattleStat stat)
    {
        AbilityLevelData levelData = data.levels[level - 1];

        stat.maxHP.multiplier += levelData.maxHPIncrease;               // 최대 체력 상승
        stat.getExp.multiplier += levelData.getEXPIncrease;             // 경험치 획득량 상승
        stat.range.multiplier += levelData.rangeIncrease;               // 범위 상승
        stat.projectileSpeed.multiplier += levelData.speedIncrease;     // 투사체 속도 상승
        stat.healHPOnSecond.additive += levelData.healHPIncrease;       // 초당 회복량 상승
        stat.duration.multiplier += levelData.durationIncrease;         // 효과 지속시간 증가
        stat.cooldown.multiplier -= levelData.cooldownDecrease;         // 공격 간격 감소
    }
}
