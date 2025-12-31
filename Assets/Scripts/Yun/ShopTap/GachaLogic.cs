using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 가챠 로직
/// 버튼 상호작용, 연출용 캔버스, UI 갱신과 분리,
/// 해당 클래스에선 핵심 가챠 로직만 작동
/// </summary>
public static class GachaLogic
{
    // 가챠테이블SO 기반으로 GachaResult 반환
    public static GachaResult DrawOnce(GachaTableSO gachaTable)
    {
        //등급 풀 결정
        Grade grade = RollGrade(gachaTable.normalRate, gachaTable.rareRate, gachaTable.legendRate);
        
        //해당등급 풀에서 Id 선택
        string itemId = RollItemIdFromPool(gachaTable, grade);
        
        //원본데이터에서 데이터 정의
        EquipData equipData = EquipDatabase.GetEquip(itemId);
        
        //런타임 데이터 객체 생성
        EquipDataRuntime equipDataRuntime = new EquipDataRuntime(equipData);
        
        //런타임 데이터를 가챠 결과 데이터 묶음으로 전달
        GachaResult result = new GachaResult();
        result.grade = grade;
        result.itemId = itemId;
        result.equipDataRuntime = equipDataRuntime;
        
        //최종 결과 반환
        return result;
    }
    /// <summary>
    /// 난수 생성해서 가챠 테이블SO의 등급별 확률대로 구간설정
    /// </summary>
    /// <param name="normalRate"></param>
    /// <param name="rareRate"></param>
    /// <param name="legendRate"></param>
    /// <returns></returns>
    private static Grade RollGrade(float normalRate, float rareRate, float legendRate)
    {
        float random = Random.value;
        
        //각 등급별 구간 설정
        if (random < normalRate) return Grade.normal;
        if (random < rareRate + normalRate) return Grade.rare;
        return Grade.legend;
    }
    /// <summary>
    /// 결정된 등급풀에서 랜덤ID 설정
    /// </summary>
    /// <param name="gachaTable"></param>
    /// <param name="grade"></param>
    /// <returns></returns>
    private static string RollItemIdFromPool(GachaTableSO gachaTable, Grade grade)
    {
        //등급에 맞는 풀
        List<string> pool = gachaTable.GetGradePool(grade);
        
        //해당 등급풀안에서 랜덤한 장비의 인덱스 선택
        int index = Random.Range(0, pool.Count);
        
        //가져온 인덱스의 Id 반환
        return pool[index];
    }

}
