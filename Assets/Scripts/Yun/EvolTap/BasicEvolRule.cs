using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 진화 관련 기본능력치 4종류씩 계속 순환되는 구조
/// 사용될 진화 데이터 정의와, 진화순환규칙 정의
/// </summary>

///진화에서 사용될 스탯종류별 데이터 정의 
public enum BasicEvolveStatType
{
    attack,
    maxHP,
    defence,
    getHPWithMeat
}

/// <summary>
/// 기본 진화 규칙
/// 1-80개 노드 기준
/// 2-레벨당 3단계씩 오픈,
/// 3-스탯 순환구조는 공격력->최대체력->방어력->고기회복량->무한루프
/// 4-진화비용은 일단 고정처리
/// 단계는 진화 슬롯 3개 포함기준
/// </summary>
public static class BasicEvolRule
{
    //총 슬롯수
    public const int totalSlot = 80;

    //진화 해금 비용
    public const int unlockEvolveCost = 1000;

    /// <summary>
    /// 플레이어 레벨당 해금가능 최대 단계
    /// -레벨1 3단계까지
    /// -레벨2 6단계까지,
    /// </summary>
    /// <param name="playerLevel"></param>
    /// <returns></returns>
    public static int GetStepCountByLevel(int playerLevel)
    {
        //해금 가능한 단계 수 저장용
        int stepCount;
        //레벨당 3단계 해금가능
        stepCount = playerLevel * 3;
        //범위 제한
        stepCount = Mathf.Clamp(stepCount, 0, totalSlot);
        return stepCount;
    }

    /// <summary>
    /// 단계별 슬롯이 올려주는 스탯타입 계산
    /// 1단계-공격력, 2단계-최대체력, 3단계 방어력, 4단계 고기회복량
    /// </summary>
    /// <param name="statStep"></param>
    /// <returns></returns>
    public static BasicEvolveStatType GetStatTypeByStep(int statStep)
    {
        //나머지값 기준으로 계산
        //나머지값 저장용
        int remainValue = (statStep - 1) % 4;
        //계산된 결과가 0이면 1단계, 1이면 2단계, 2면 3단계, 3이면 4단계 스탯을 적용
        if (remainValue == 0) return BasicEvolveStatType.attack;
        if (remainValue == 1) return BasicEvolveStatType.maxHP;
        if (remainValue == 2) return BasicEvolveStatType.defence;
        return BasicEvolveStatType.getHPWithMeat;
    }

    /// <summary>
    /// 각 단계별 올려주는 스탯량 - 임시로 고정
    /// </summary>
    /// <returns></returns>
    public static int GetIncreaseAmountByStep()
    {
        int amount;
        //일단 고정수치 전부 10 주는데, 테이블 따로 생기면 여기 변경할 것
        amount = 10; 
        return amount;
    }

    /// <summary>
    /// 스탯타입에 따른 스탯이름 텍스트로 변환
    /// </summary>
    /// <param name="statType"></param>
    /// <returns></returns>
    public static string GetStatNameToText(BasicEvolveStatType statType)
    {
        if (statType == BasicEvolveStatType.attack) return "공격력";
        if (statType == BasicEvolveStatType.maxHP) return "최대체력";
        if (statType == BasicEvolveStatType.defence) return "방어력";
        return "고기회복량";
    }
}
