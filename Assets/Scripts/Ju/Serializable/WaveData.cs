using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveData : MonoBehaviour
{
    public int startTime;           // 웨이브 시작 시간
    public int endTime;             // 웨이브 종료 시간
    public EnemyDatabase enemies;   // 웨이브 동안 스폰되는 적 데이터베이스
    public float spawnRate;         // 스폰 간격
    public int spawnCount;          // 스폰되는 수
}