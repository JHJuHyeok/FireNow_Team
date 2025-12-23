using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 패시브 획득 시 생성할 인스턴스
public class Passive
{
    public string passiveId { get; private set; }
    public int level { get; private set; }
    public AbilityDataRuntime data { get; private set; }
    public PassiveProvider provider { get; private set; }

    /// <summary>
    /// 패시브 생성자
    /// </summary>
    /// <param name="data"> 관련 어빌리티 런타임 데이터 </param>
    /// <param name="level"> 패시브 레벨 </param>
    public Passive(AbilityDataRuntime data, int level = 1)
    {
        this.data = data;
        this.level = level;
        provider = new PassiveProvider(data, level);
    }

    /// <summary>
    /// 패시브 레벨업
    /// </summary>
    public void LevelUp()
    {
        level++;
        provider.SetLevel(level);
    }
}
