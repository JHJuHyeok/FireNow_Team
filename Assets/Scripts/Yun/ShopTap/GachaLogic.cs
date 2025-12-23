using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 가챠 로직 그잡채
/// 버튼 상호작용, 연출용 캔버스, UI 갱신과 분리,
/// 여기선 가챠 로직만을 구현하는게 목적
/// </summary>
public static class GachaLogic
{
    //아까 만들어둔 가챠테이블SO 설정 기반으로 GachaResult 결과 반환
    public static GachaResult DrawOnce(GachaTableSO gachaTable)
    {
        //확률로 등급 풀 결정
        Grade grade = RollGrade(gachaTable.normalRate, gachaTable.rareRate, gachaTable.legendRate);
        //해당등급 풀에서 Id 선택
        string itemId = RollItemIdFromPool(gachaTable, grade);
        //이제 가져온 Id를 원본데이터에서 가져와야 됨
        EquipData equipData = EquipDatabase.GetEquip(itemId);
        //원본데이터 정보로 런타임 데이터 객체 생성
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
        //노말구간 설정
        if (random < normalRate) return Grade.normal;
        //레어구간 설정
        if (random < rareRate + normalRate) return Grade.rare;
        //레전드 구간 설정(나머지)
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
        //등급에 맞는 풀 일단 가져오기
        List<string> pool = gachaTable.GetGradePool(grade);
        //그 등급풀안에서 랜덤한 장비의 인덱스 선택
        int index = Random.Range(0, pool.Count);
        //가져온 인덱스의 Id 반환
        return pool[index];
    }

}
