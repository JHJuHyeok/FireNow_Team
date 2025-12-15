using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EvolveData
{
    public string id;               // 진화 ID
    public string evolveName;       // 진화 이름
    public string iconPath;         // 아이콘 경로
    public string descript;         // 진화 설명
}

[System.Serializable]
public class EvolveDatabase
{
    public List<EvolveData> evolves;
}