using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 이니셜라이즈 타이밍이 늦게 일어나고 있는지 확인하기 위한 부트스트랩
/// 첫씬에 전체 데이터베이스, 테이블 초기화 보장
/// </summary>
public class GameBootStrap : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Initializer.InitializeAllData();
    }
}
