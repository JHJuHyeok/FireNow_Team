using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StuffData
{
    public string id;           // 소지품 ID
    public string stuffName;    // 소지품 이름
    public string spriteName;   // 아틀라스에서 불러올 스프라이트 이름
    public string descript;     // 소지품 설명

    public Grade grade;         // 소지품 등급
}

[System.Serializable]
public class StuffDatabaseDTO
{
    public List<StuffData> stuffs;
}