using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExperience : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int expToNextLevel = 100;
    [SerializeField] private float expMultiplier = 1.5f;

    [Header("UI")]
    [SerializeField] private Slider expBar;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject levelUpUI; // 레벨업 UI (기존)

    [Header("Ability Selection")]
    [SerializeField] private AbilitySelectionManager abilitySelectionManager; // 추가!

    public event Action OnLevelUp;

    private void Start()
    {
        UpdateUI();
    }

    public void AddExperience(int amount)
    {
        currentExp += amount;

        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (expBar != null)
        {
            expBar.maxValue = expToNextLevel;
            expBar.value = currentExp;
        }

        if (levelText != null)
        {
            levelText.text = $"{currentLevel}";
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExp -= expToNextLevel;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * expMultiplier);

        OnLevelUp?.Invoke();


     


        // 능력 선택 UI 표시 (추가!)
        if (abilitySelectionManager != null)
        {

      
            abilitySelectionManager.ShowAbilitySelection();
            Time.timeScale = 0f; // 게임 일시정지
        }
        // 기존 레벨업 UI는 제거하거나 능력 선택 UI와 통합
        else if (levelUpUI != null)
        {

            Debug.Log($"  레벨 : {currentLevel}");
            Debug.Log($" 경험치: {currentExp}");
            levelUpUI.SetActive(true);
            Time.timeScale = 0f;
        }

        UpdateUI();
    }

    public int GetLevel() => currentLevel;
    public int GetCurrentExp() => currentExp;
    public int GetExpToNextLevel() => expToNextLevel;
}