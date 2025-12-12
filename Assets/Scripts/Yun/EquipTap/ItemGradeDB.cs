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
    public Sprite GetBorder(ItemGrade grade)
    {
        switch (grade)
        {
            case ItemGrade.Normal: return normalBorder;
            case ItemGrade.Rare: return rereBorder;
            case ItemGrade.Legendary: return LegendaryBorder;
        }
        return null;
    }

    ///등급별 상단 이미지 호출 함수
    public Sprite GetTopImage(ItemGrade grade)
    {
        switch (grade)
        {
            case ItemGrade.Normal: return normalTopImage;
            case ItemGrade.Rare:return rereTopImage;
            case ItemGrade.Legendary:return LegendaryTopImage;
        }
        return null;
    }
}
