using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EvolveData
{
    public string id;                   // 진화 ID
    public string evolveName;           // 진화 이름
    public EvolveNodeType nodeType;     // 노드 타입
    public string gainStat;             // 성장하는 능력치
    public string activeSpriteName;     // 아틀라스에서 불러올 스프라이트 이름
    public string deactiveSpriteName;   // 비활성화 스프라이트 이름
    public string descript;             // 진화 설명
}

[System.Serializable]
public class EvolveDatabaseDTO
{
    public List<EvolveData> evolves;
}

[System.Serializable]
public class EvolveLevelConfigs
{
    public List<EvolveLevelConfig> levels;
}

[System.Serializable]
public class EvolveLevelConfig
{
    public int level;                   // 레벨
    public int cost;                    // 레벨 별 금액
    public List<EvolveConfig> configs;  // 내부 진화 노드 요소
}

[System.Serializable]
public class EvolveConfig
{
    public string evolveId;
    public int value;
}

public enum EvolveNodeType
{
    normal,
    special
}