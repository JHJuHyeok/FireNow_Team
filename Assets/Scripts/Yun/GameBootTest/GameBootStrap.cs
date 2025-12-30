using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 이니셜라이즈 타이밍이 늦게 일어나고 있는지 확인하기 위한 부트스트랩
/// 첫씬에 전체 데이터베이스, 테이블 초기화 보장
/// </summary>
public class GameBootStrap : MonoBehaviour
{
    [Header("씬에 배치된 SaveMaager 인스턴스")]
    [SerializeField] private SaveManager saveManager;
    [Header("SoundManager 인스턴스")]
    [SerializeField] private SoundManager soundManager;

    private float bgmVolume = -10f;
    private float effectVolume = -20f;

    private void Awake()
    {
        //DDL 깔고
        DontDestroyOnLoad(gameObject);
        //테이블 및 데이터베이스 초기화
        Initializer.InitializeAllData();
        //세이브 매니저가 인스펙터에 연결되어 있으면 그 인스턴스로 로드
        if (saveManager != null)
        {
            //씬에 있는 세이브매니저를 직접 참조해서 로드
            saveManager.Load();
        }
        // 사운드 볼륨 초기화
        soundManager.InitVolumes(bgmVolume, effectVolume);
    }
}
