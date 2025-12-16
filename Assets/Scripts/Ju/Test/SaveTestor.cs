using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTestor : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;

    private void Awake()
    {
        // 데이터베이스 불러오기
        EquipDatabase.Initialize();
        StuffDatabase.Initialize();
        StageDatabase.Initialize();

        // 저장된 데이터 불러오기
        saveManager.Load();
    }

    private void Start()
    {
        
    }
}
