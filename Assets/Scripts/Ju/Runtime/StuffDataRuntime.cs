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
    public Sprite icon;         // 아이콘 경로
    public string descript;     // 소지품 설명

    public Grade grade;         // 소지품 등급

    /// <summary>
    /// Json 데이터를 입력받아 런타임 소지품 데이터에 대입시키는 함수
    /// </summary>
    /// <param name="data"> 소지품 JSON 데이터</param>
    public StuffDataRuntime(StuffData data)
    {
        id = data.id;
        stuffName = data.stuffName;
        icon = Resources.Load<Sprite>(data.spriteName);
        descript = data.descript;

        grade = data.grade;
    }
}
