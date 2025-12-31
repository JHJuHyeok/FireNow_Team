using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 가챠 테이블 정할 SO
/// 가챠에 필요한 비용, 들어갈 확률, 등급별 아이템 풀을 데이터로 관리하기 위함
/// </summary>
[CreateAssetMenu(fileName = "GachaTableSO", menuName ="Shop/GachaTable")]
public class GachaTableSO : ScriptableObject
{
    [Header("가챠 1회에 필요 비용")]
    public int gemCost = 80;

    //합계 1이 되도록 조정할 것
    [Header("등급별 확률 조정")]
    [Range(0.0f, 1.0f)] public float normalRate = 0.5f;
    [Range(0.0f, 1.0f)] public float rareRate = 0.3f;
    [Range(0.0f, 1.0f)] public float legendRate = 0.2f;

    [Header("등급별 아이템 풀")]
    [SerializeField] private List<string> normalPool = new List<string>();
    [SerializeField] private List<string> rarePool = new List<string>();
    [SerializeField] private List<string> legendPool = new List<string>();

    /// <summary>
    /// 등급에 맞는 ID 풀 반환용
    /// </summary>
    /// <param name="grade"></param>
    /// <returns></returns>
    public List<string> GetGradePool(Grade grade)
    {
        //여기서 등급분기 해주기
        switch (grade)
        {
            case Grade.normal: return normalPool;
            case Grade.rare: return rarePool;
            case Grade.legend: return legendPool;
        }
        return null;
    }
}
