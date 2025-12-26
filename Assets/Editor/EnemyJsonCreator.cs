using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EnemyJsonCreator : EditorWindow
{
    string id;
    List<EnemyData> enemies = new List<EnemyData>();

    [MenuItem("Tools/JSON/Enemy JSON Creator")]
    public static void ShowWindow()
    {
        GetWindow<EnemyJsonCreator>("EnemyDatabase Creator");
    }

    [SerializeField] private Vector2 scrollPos = Vector2.zero;
    [SerializeField] bool boolBar = true;

    private void OnGUI()
    {
        GUILayout.Label("Enemy 데이터베이스", EditorStyles.boldLabel);

        id = EditorGUILayout.TextField("데이터베이스 ID", id);

        int enemyCount = Mathf.Max(0, EditorGUILayout.IntField("Enemies Count", enemies.Count));

        while (enemyCount > enemies.Count)
            enemies.Add(new EnemyData());
        while (enemyCount < enemies.Count)
            enemies.RemoveAt(enemies.Count - 1);

        GUILayout.Space(8);

        boolBar = EditorGUILayout.Foldout(boolBar, "몬스터 리스트");

        if (boolBar)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            for (int i = 0; i < enemies.Count; i++)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label($"Enemy {i + 1}", EditorStyles.boldLabel);

                GUILayout.Label("Enemy JSON 기본 정보", EditorStyles.boldLabel);

                enemies[i].id = EditorGUILayout.TextField("몬스터 ID", enemies[i].id);
                enemies[i].hp = EditorGUILayout.IntField("몬스터 체력", enemies[i].hp);
                enemies[i].speed = EditorGUILayout.FloatField("몬스터 속도", enemies[i].speed);
                enemies[i].damage = EditorGUILayout.FloatField("몬스터 공격력", enemies[i].damage);
                enemies[i].dropItem = EditorGUILayout.TextField("드랍 아이템 ID", enemies[i].dropItem);

                GUILayout.Space(10);
                GUILayout.Label("Enemy 애니메이션 경로", EditorStyles.boldLabel);

                enemies[i].idleAnimation = EditorGUILayout.TextField("통상 애니메이션", enemies[i].idleAnimation);
                enemies[i].deadAnimation = EditorGUILayout.TextField("사망 애니메이션", enemies[i].deadAnimation);

                GUILayout.Space(10);
                GUILayout.Label("Enemy 이동 방식", EditorStyles.boldLabel);

                enemies[i].moveType = (MoveType)EditorGUILayout.EnumPopup("이동 방식", enemies[i].moveType);

                GUILayout.Space(5);

                if (GUILayout.Button("Remove This Enemy"))
                {
                    enemies.RemoveAt(i);
                    break;
                }

                GUILayout.EndVertical();
                GUILayout.Space(4);
            }
            EditorGUILayout.EndScrollView();
        }

        if (GUILayout.Button("Add new Enemy"))
        {
            enemies.Add(new EnemyData());
        }

        GUILayout.Space(15);

        if (GUILayout.Button("Json 파일 생성"))
        {
            CreateEnemyJson();
        }
    }

    private void CreateEnemyJson()
    {
        EnemyDatabaseDTO data = new EnemyDatabaseDTO
        {
            id = id,
            enemyList = enemies
        };

        string json = JsonUtility.ToJson(data, true);

        string folder = "Assets/Resources/Json/Enemy";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string filePath = $"{folder}/{id}.json";
        File.WriteAllText(filePath, json);

        AssetDatabase.Refresh();
        Debug.Log($"생성 완료: {filePath}");
    }
}
