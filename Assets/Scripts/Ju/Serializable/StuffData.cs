using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StuffData
{
    public string id;           // 소지품 ID
    public string stuffName;    // 소지품 이름
    public string iconPath;     // 아이콘 경로
    public string descript;     // 소지품 설명

    public Grade grade;         // 소지품 등급
}

[System.Serializable]
public class StuffDatabase
{
    public List<StuffData> stuffs;
}