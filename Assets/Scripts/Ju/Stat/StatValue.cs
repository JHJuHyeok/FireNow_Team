using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 모든 스탯의 기본 형태
[System.Serializable]
public class StatValue
{
    public float baseValue;         // 기본값
    public float additive;          // +
    public float multiplier;        // *

    // (기본값 + 추가 스탯) * (1f * 퍼센티지)
    public float calculateValue
    {
        get { return (baseValue + additive) * (1f + multiplier); }
    }

    public void ClearStat()
    {
        additive = 0f;
        multiplier = 0f;
    }
}