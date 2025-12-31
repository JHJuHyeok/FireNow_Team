using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 가챠 결과 데이터
/// 가챠 결과창에서 필요한 정보(등급,아이템이름,아이콘 등)를 데이터 묶음으로 전달하기 위함
/// </summary>
public class GachaResult
{
    //뽑힌 장비 등급
    public Grade grade;
    
    //뽑힌 장비 ID
    public string itemId;
    
    //런타임 기준 장비 데이터
    public EquipDataRuntime equipDataRuntime;
    
    //뽑힌 장비 이름
    public string ItemName
    {
        get { return equipDataRuntime.equipName; }
    }
    
    //뽑힌 장비 아이콘
    public Sprite ItemIcon
    {
        get { return equipDataRuntime.icon; }
    }
}
