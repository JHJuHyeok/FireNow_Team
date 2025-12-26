using System.Collections.Generic;
using UnityEngine;

// ========== 재화 타입 ==========
public enum CurrencyType
{
    Gold,      // 골드
    GoldDNA    // 골드 DNA
}

// ========== 재화 획득 방법 ==========
[System.Serializable]
public class CurrencySource
{
    public string sourceName;        // 획득처 이름 (예: "스테이지 클리어")
}

// ========== 스킬 타입 데이터 (기본 4가지) ==========
[System.Serializable]
public class SkillTypeData
{
    [Header("기본 정보")]
    public string skillName;         // "힘", "체력", "끈기", "회복"
    public Sprite lockedSprite;      // 잠금 이미지
    public Sprite unlockedSprite;    // 해제 이미지

    [Header("스탯 정보")]
    public string statType;          // "공격력", "HP", "방어구", "고기회복"
    public int statIncreasePerLevel; // 레벨당 증가량

    [Header("비용 정보")]
    public CurrencyType requiredCurrency;  // 필요한 재화 종류
    public int baseCost;                   // 기본 비용
    public float costIncreaseRate;         // 비용 증가율 (예: 1.1 = 10%씩 증가)

    [Header("재화 획득처")]
    public List<CurrencySource> sources = new List<CurrencySource>();   // 재화 획득 방법 리스트
}

// ========== 특수 스킬 데이터 ==========
[System.Serializable]
public class SpecialSkillData
{
    [Header("기본 정보")]
    public string skillName;         // 특수 스킬 이름
    public Sprite lockedSprite;      // 잠금 이미지
    public Sprite unlockedSprite;    // 해제 이미지
    public string description;       // 스킬 설명

    [Header("해금 조건")]
    public int requiredLevel;        // 필요한 레벨 (예: 10레벨에 해금)

    [Header("비용 정보")]
    public CurrencyType requiredCurrency;
    public int cost;                 // 고정 비용

    [Header("재화 획득처")]
    public List<CurrencySource> sources = new List<CurrencySource>();
}

// ========== 플레이어 진행 상황 ==========
[System.Serializable]
public class PlayerProgress
{
    [Header("현재 진행도")]
    public int currentLevel = 1;         // 플레이어 레벨 (1~100)

    [Header("구매 완료 여부")]
    public bool[] purchasedSkills = new bool[300];   // 각 스킬의 구매 여부 
    public bool[] purchasedSpecialSkills = new bool[10]; // 특수 스킬 구매 여부

    // 현재 레벨에서 해금 가능한 최대 스킬 개수
    public int GetMaxUnlockedSkills()
    {
        return currentLevel * 3;
    }

    // 구매한 스킬 개수
    public int GetPurchasedSkillCount()
    {
        int count = 0;
        for (int i = 0; i < purchasedSkills.Length; i++)
        {
            if (purchasedSkills[i]) count++;
        }
        return count;
    }

    // 특정 스킬 인덱스가 구매되었는지 확인
    public bool IsSkillPurchased(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= purchasedSkills.Length) return false;
        return purchasedSkills[skillIndex];
    }

    // 스킬 구매 처리
    public void PurchaseSkill(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= purchasedSkills.Length) return;
        purchasedSkills[skillIndex] = true;
    }

    // 특수 스킬 구매 처리
    public void PurchaseSpecialSkill(int specialSkillIndex)
    {
        if (specialSkillIndex < 0 || specialSkillIndex >= purchasedSpecialSkills.Length) return;
        purchasedSpecialSkills[specialSkillIndex] = true;
    }

    // 특수 스킬 구매 여부 확인
    public bool IsSpecialSkillPurchased(int specialSkillIndex)
    {
        if (specialSkillIndex < 0 || specialSkillIndex >= purchasedSpecialSkills.Length) return false;
        return purchasedSpecialSkills[specialSkillIndex];
    }

    // 스킬 인덱스에서 레벨 계산
    public int GetLevelFromSkillIndex(int skillIndex)
    {
        return (skillIndex / 3) + 1;
    }

    // 레벨에서 시작 스킬 인덱스 계산
    public int GetStartSkillIndexFromLevel(int level)
    {
        return (level - 1) * 3;
    }
}