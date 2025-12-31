using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 상단 HUD 전용 스크립트->표시할 항목(스태미나/젬/골드)
/// HUD의 값이 변경되는 곳에서 호출
/// 재화 축약표현 K,M 까지 포함
/// </summary>
public class HUD : MonoBehaviour
{
    [Header("플레이어 데이터")]
    [SerializeField] private PlayerInfoSO playerInfo;

    [Header("표시 될 HUD 텍스트")]
    [SerializeField] private TextMeshProUGUI haveGoldText;
    [SerializeField] private TextMeshProUGUI haveGemText;
    [SerializeField] private TextMeshProUGUI haveStaminaText;
    [SerializeField] private TextMeshProUGUI maxStaminaText;

    private void Start()
    {
        SoundManager.Instance.PlaySound("MainMenu_BGM", 0, true, SoundType.bgm);
    }

    /// <summary>
    /// 메인메뉴 들어올 때 호출
    /// </summary>
    private void OnEnable()
    {
        RefreshHUD(playerInfo);
    }

    /// <summary>
    /// 외부에서 호출 할 HUD 갱신 함수 아
    /// </summary>
    public void RefreshHUD(PlayerInfoSO playerInfo)
    {
        haveGoldText.text = ChangeTextUnit(playerInfo.gold);
        haveGemText.text = ChangeTextUnit(playerInfo.gem);
        haveStaminaText.text = ChangeTextUnit(playerInfo.stamina);
        maxStaminaText.text = ChangeTextUnit(playerInfo.maxStamina);
    }

    /// <summary>
    /// HUD재화관련 UI 축약표현
    /// k=천 단위 m=백만 단위
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private string ChangeTextUnit(int value)
    {
        //1000 아래일땐 그대로 표현
        if (value < 1000)
        {
            return value.ToString();
        }
        //value가 1,000,000 아래일때(소수점 표현)
        if (value < 1000000)
        {
            float kValue = value / 1000f;
            return DecimalPoint(kValue) + "K";
        }
        
        float mValue = value / 1000000f;
        return DecimalPoint(mValue) + "M";
    }

    /// <summary>
    /// 소수점 표현
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private string DecimalPoint(float value)
    {
        //기본 문자열 형식은 소수점 표현된 상태로
        string text = value.ToString("0.0");
        
        //소수점이 정확하게.0이면
        if (text.EndsWith(".0"))
        {
            //뒤에 2자리는 제거
            text = text.Substring(0, text.Length - 2);
        }
        return text;
    }
}
