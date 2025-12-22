using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDataRuntime
{
    public string id;                           // 아이템 식별 ID
    public string name;                         // UI에 표시될 이름
    public Sprite sprite;                       // 아틀라스에서 불러올 스프라이트 이름
    public int maxLevel;                        // 최대 레벨

    public AbilityType type;                    // 아이템의 타입

    public List<AbilityLevelData> levels;       // 각 레벨의 데이터

    public WeaponEvolution evolution;           // 진화 관련 데이터

    /// <summary>
    /// Json 데이터를 입력받아 런타임 어빌리티 데이터에 대입시키는 함수
    /// </summary>
    /// <param name="data"> 어빌리티 JSON 데이터</param>
    public AbilityDataRuntime(AbilityData data)
    {
        id = data.id;
        name = data.name;
        sprite = AtlasManager.GetSprite("", data.spriteName);
        maxLevel = data.maxLevel;

        type = data.type;
        levels = data.levels;
        evolution = data.evolution;
    }
}
