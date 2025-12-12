using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ItemJsonCreator : EditorWindow
{
    string id;
    string itemName;
    string itemSprite;
    bool magnet = true;
    int expValue;

    [MenuItem("Tools/Item JSON Creator")]
    public static void ShowAbilityWindow()
    {
        GetWindow<ItemJsonCreator>("Item Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Item JSON 기본 정보", EditorStyles.boldLabel);

        id = EditorGUILayout.TextField("아이템 ID", id);
        itemName = EditorGUILayout.TextField("아이템 이름", itemName);
        itemSprite = EditorGUILayout.TextField("이미지 경로", itemSprite);

        GUILayout.Space(10);
        GUILayout.Label("Item 추가 정보", EditorStyles.boldLabel);

        magnet = EditorGUILayout.Toggle("자석 효과 여부", magnet);
        expValue = EditorGUILayout.IntField("경험치 획득량", expValue);

        GUILayout.Space(15);

        if (GUILayout.Button("Json 파일 생성"))
        {
            CreateItemJson();
        }
    }

    private void CreateItemJson()
    {
        ItemData data = new ItemData
        {
            id = id,
            itemName = itemName,
            itemSprite = itemSprite,
            magnet = magnet,
            expValue = expValue
        };

        string json = JsonUtility.ToJson(data, true);

        string folder = "Assets/Resources/Json";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string filePath = $"{folder}/{id}.json";
        File.WriteAllText(filePath, json);

        AssetDatabase.Refresh();
        Debug.Log($"생성 완료: {filePath}");
    }
}
