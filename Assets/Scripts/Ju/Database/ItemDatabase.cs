using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase
{
    private static Dictionary<string, ItemData> itemDict;

    /// <summary>
    /// 게임 시작 시 드롭아이템 데이터베이스 불러오기
    /// </summary>
    public static void Initialize()
    {
        itemDict = new Dictionary<string, ItemData>();

        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("Json/Item");

        foreach (var file in jsonFiles)
        {
            ItemData data = JsonUtility.FromJson<ItemData>(file.text);
            itemDict[data.id] = data;
        }
    }

    /// <summary>
    /// 드롭 아이템 데이터 불러오기
    /// </summary>
    /// <param name="id"> 불러올 아이템 ID </param>
    /// <returns> ID값에 대응하는 ItemData 값 (없으면 null 반환) </returns>
    public static ItemData GetAbility(string id)
    {
        if (itemDict.TryGetValue(id, out var item))
            return item;

        return null;
    }
}
