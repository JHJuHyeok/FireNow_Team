using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//아이템 등급값에 맞게 이미지 맵핑SO

[CreateAssetMenu (fileName = "ItemGradeDB", menuName = "Item/Grade DataBase")]
public class ItemGradeDB : ScriptableObject
{
    [Header("등급별 테두리")]
    public Sprite normalBorder;
    public Sprite rereBorder;
    public Sprite LegendaryBorder;

    [Header("등급별 상단 이미지")]
    public Sprite normalTopImage;
    public Sprite rereTopImage;
    public Sprite LegendaryTopImage;

    /// <summary>
    /// 등급별 테두리 이미지 호출 함수
    /// </summary>
    /// <param name="grade"></param>
    /// <returns></returns>
    public Sprite GetBorder(Grade grade)
    {
        switch (grade)
        {
            case Grade.normal: return normalBorder;
            case Grade.rare: return rereBorder;
            case Grade.legend: return LegendaryBorder;
        }
        return null;
    }

    ///등급별 상단 이미지 호출 함수
    public Sprite GetTopImage(Grade grade)
    {
        switch (grade)
        {
            case Grade.normal: return normalTopImage;
            case Grade.rare:return rereTopImage;
            case Grade.legend:return LegendaryTopImage;
        }
        return null;
    }
}
