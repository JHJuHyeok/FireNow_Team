using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EquipJsonCreator : EditorWindow
{
    string id;
    string equipName;
    string descript;
    string spriteName;

    EquipPart part = EquipPart.weapon;

    List<EquipGrade> equipGrades = new List<EquipGrade>();

    [MenuItem("Tools/JSON/Equip JSON Creator")]
    public static void ShowWindow()
    {
        GetWindow<EquipJsonCreator>("Equip Creator");
    }

    [SerializeField] private Vector2 scrollPos = Vector2.zero;
    [SerializeField] bool gradeBar = true;

    private void OnGUI()
    {
        GUILayout.Label("Equip JSON 기본 정보", EditorStyles.boldLabel);

        id = EditorGUILayout.TextField("ID", id);
        equipName = EditorGUILayout.TextField("이름", equipName);
        descript = EditorGUILayout.TextField("장비 설명", descript);
        spriteName = EditorGUILayout.TextField("스프라이트 명칭", spriteName);

        part = (EquipPart)EditorGUILayout.EnumPopup("타입", part);

        GUILayout.Space(5);

        int gradeCount = Mathf.Max(0, EditorGUILayout.IntField("Grade Allign", equipGrades.Count));

        while (gradeCount > equipGrades.Count)
            equipGrades.Add(new EquipGrade());
        while (gradeCount < equipGrades.Count)
            equipGrades.RemoveAt(equipGrades.Count - 1);

        gradeBar = EditorGUILayout.Foldout(gradeBar, "등급 별 설명");
        if (gradeBar)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            for (int i = 0; i < equipGrades.Count; i++)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label($"{i + 1}번째 등급", EditorStyles.boldLabel);

                equipGrades[i].grade = (Grade)EditorGUILayout.EnumPopup("등급", equipGrades[i].grade);
                equipGrades[i].maxLevel = EditorGUILayout.IntField("최대 레벨", equipGrades[i].maxLevel);
                equipGrades[i].startValue = EditorGUILayout.IntField("초기 능력치", equipGrades[i].startValue);
                equipGrades[i].descript = EditorGUILayout.TextField("등급별 능력", equipGrades[i].descript);

                GUILayout.Space(5);

                if (GUILayout.Button("Remove This Grade"))
                {
                    equipGrades.RemoveAt(i);
                    break;
                }

                GUILayout.EndVertical();
                GUILayout.Space(4);
            }

            EditorGUILayout.EndScrollView();
        }

        if (GUILayout.Button("Add new Grade"))
        {
            equipGrades.Add(new EquipGrade());
        }

        GUILayout.Space(15);

        if (GUILayout.Button("Json 파일 생성"))
        {
            CreateEquipJson();
        }
    }

    void CreateEquipJson()
    {
        EquipData data = new EquipData
        {
            id = id,
            equipName = equipName,
            descript = descript,
            spriteName = spriteName,
            part = part,
            equipGrades = equipGrades
        };

        string json = JsonUtility.ToJson(data, true);

        string folder = "Assets/Resources/Json/Equip";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string filePath = $"{folder}/{id}.json";
        File.WriteAllText(filePath, json);

        AssetDatabase.Refresh();
        Debug.Log($"생성 완료: {filePath}");
    }
}
