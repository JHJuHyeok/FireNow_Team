using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 런타임 중 쓰일 소지품 데이터
/// </summary>
public class StuffDataRuntime
{
    public string id;           // 소지품 ID
    public string stuffName;    // 소지품 이름
    public Sprite icon;     // 아이콘 경로
    public string descript;     // 소지품 설명

    public Grade grade;         // 소지품 등급

    public StuffDataRuntime(StuffData data)
    {
        id = data.id;
        stuffName = data.stuffName;
        icon = Resources.Load<Sprite>(data.iconPath);
        descript = data.descript;

        grade = data.grade;
    }
}
