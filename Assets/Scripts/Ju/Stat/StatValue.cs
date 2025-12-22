using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatValue
{
    public float baseValue;
    public float additive;
    public float multiplier;

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