using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveManager
{
    private Dictionary<string, Passive> passiveDict = new();

    /// <summary>
    /// 패시브 획득(레벨 업)시 딕셔너리에 추가
    /// </summary>
    /// <param name="data"> 패시브의 어빌리티 런타임 데이터 </param>
    public void AddPassive(AbilityDataRuntime data)
    {
        // 이미 지니고 있는 패시브면 레벨업
        if(passiveDict.TryGetValue(data.id, out var passive))
        {
            passive.LevelUp();
        }
        // 지니고 있지 않으면 새로 생성
        else
        {
            passiveDict[data.id] = new Passive(data);
        }
    }

    /// <summary>
    /// 전투 시 스탯에 지니고 있는 패시브 적용
    /// </summary>
    /// <param name="stat"> 전투 시 스탯 </param>
    public void ApplyAll(BattleStat stat)
    {
        stat.ClearRuntimeStats();

        foreach(var passive in passiveDict.Values)
        {
            passive.provider.Apply(stat);
        }

        stat.Refresh();
    }
}
