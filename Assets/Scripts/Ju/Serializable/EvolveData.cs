using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EvolveData
{
    public string id;               // 진화 ID
    public string evolveName;       // 진화 이름
    public string spriteName;       // 아틀라스에서 불러올 스프라이트 이름
    public string descript;         // 진화 설명
}

[System.Serializable]
public class EvolveDatabaseDTO
{
    public List<EvolveData> evolves;
}