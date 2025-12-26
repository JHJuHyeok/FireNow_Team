using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string id;           // 아이템 ID
    public string itemName;     // 아이템 이름
    public string spriteName;   // 아틀라스에서 불러올 스프라이트 이름
    public bool magnet;         // 자석 효과 적용 여부
    public int expValue;        // 획득하는 경험치
}