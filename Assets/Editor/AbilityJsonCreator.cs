using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AbilityJsonCreator : EditorWindow
{
    string id = "";
    string abilityName = "";
    string icon = "";
    int maxLevel = 5;

    AbilityType type = AbilityType.weapon;
    List<AbilityLevelData> levels = new List<AbilityLevelData>();

    WeaponEvolution evolution = new WeaponEvolution();

    [MenuItem("Tools/Ability JSON Creator")]
    public static void ShowWindow()
    {
        GetWindow<AbilityJsonCreator>("Ability Creator");
    }

    [SerializeField] private Vector2 scrollPos = Vector2.zero;
    [SerializeField] bool levelBar = true;
    string status = "각 레벨별 능력치 추가";

    private void OnGUI()
    {
        GUILayout.Label("Ability JSON 기본 정보", EditorStyles.boldLabel);

        id = EditorGUILayout.TextField("ID", id);
        abilityName = EditorGUILayout.TextField("이름", abilityName);
        icon = EditorGUILayout.TextField("아이콘 경로", icon);
        maxLevel = EditorGUILayout.IntField("최대 레벨", maxLevel);

        type = (AbilityType)EditorGUILayout.EnumPopup("타입", type);

        GUILayout.Space(5);

        GUILayout.BeginVertical("box");
        GUILayout.Label($"진화 데이터", EditorStyles.boldLabel);

        evolution.requireItem = EditorGUILayout.TextField("필요 아이템", evolution.requireItem);
        evolution.result = EditorGUILayout.TextField("진화 결과", evolution.result);

        GUILayout.EndVertical();

        GUILayout.Space(5);

        GUILayout.Label("레벨 리스트", EditorStyles.boldLabel);

        int levelCount = Mathf.Max(0, EditorGUILayout.IntField("Levels Count", levels.Count));

        while (levelCount > levels.Count)
            levels.Add(new AbilityLevelData());
        while (levelCount < levels.Count)
            levels.RemoveAt(levels.Count - 1);

        GUILayout.Space(5);

        levelBar = EditorGUILayout.Foldout(levelBar, status);
        if (levelBar)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            for (int i = 0; i < levels.Count; i++)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label($"Level {i + 1}", EditorStyles.boldLabel);

                GUILayout.Label("액티브", EditorStyles.miniBoldLabel);
                levels[i].damage = EditorGUILayout.FloatField("데미지", levels[i].damage);
                levels[i].cooldown = EditorGUILayout.FloatField("쿨다운", levels[i].cooldown);
                levels[i].range = EditorGUILayout.FloatField("범위", levels[i].range);
                levels[i].speed = EditorGUILayout.FloatField("속도", levels[i].speed);
                levels[i].duration = EditorGUILayout.FloatField("지속 시간", levels[i].duration);
                levels[i].projectileCount = EditorGUILayout.IntField("투사체 수", levels[i].projectileCount);

                GUILayout.Label("패시브", EditorStyles.miniBoldLabel);
                levels[i].rangeIncrease = EditorGUILayout.FloatField("범위 상승치", levels[i].rangeIncrease);
                levels[i].speedIncrease = EditorGUILayout.FloatField("투사체 속도 상승치", levels[i].speedIncrease);
                levels[i].maxHPIncrease = EditorGUILayout.FloatField("최대 체력 상승치", levels[i].maxHPIncrease);
                levels[i].healHPIncrease = EditorGUILayout.FloatField("초당 회복량 상승치", levels[i].healHPIncrease);
                levels[i].durationIncrease = EditorGUILayout.FloatField("지속시간 상승치", levels[i].durationIncrease);
                levels[i].cooldownDecrease = EditorGUILayout.FloatField("공격 간격 감소치", levels[i].cooldownDecrease);
                levels[i].getEXPIncrease = EditorGUILayout.FloatField("경험치 획득량 상승치", levels[i].getEXPIncrease);

                GUILayout.Label("설명", EditorStyles.miniBoldLabel);
                levels[i].description = EditorGUILayout.TextField("설명", levels[i].description);

                GUILayout.Space(5);

                if (GUILayout.Button("Remove This Level"))
                {
                    levels.RemoveAt(i);
                    break;
                }

                GUILayout.EndVertical();
                GUILayout.Space(4);
            }

            EditorGUILayout.EndScrollView();
        }

        if(GUILayout.Button("Add new Level"))
        {
            levels.Add(new AbilityLevelData());
        }

        GUILayout.Space(15);

        if (GUILayout.Button("Json 파일 생성"))
        {
            CreateAbilityJson();
        }
    }

    void CreateAbilityJson()
    {
        AbilityData data = new AbilityData
        {
            id = id,
            name = abilityName,
            icon = icon,
            maxLevel = maxLevel,
            type = type,
            evolution = evolution,
            levels = levels
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
