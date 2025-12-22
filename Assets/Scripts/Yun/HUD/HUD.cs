using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 상단 HUD 전용 스크립트->표시할 항목(스태미나/젬/골드)
/// HUD의 값이 변경되는 곳에서 호출 할 것
/// 변경되는 타이밍 -> 필요 상황 모두 기억할것-
/// -1.게임시작 버튼(스태미나 소모)
/// -2.시간에 따른 스태미나 재생(스태미나 획득) -보니까 UTC 기준으로 시간 저장하고, 5분마다 1개씩 늘어나는 구조인듯?
/// 그러면 나도 5분마다 갱신을 해줘야되나?????????오바인데; 캡은 걸려있고, 딱히 신경 안써도 되나?
/// 5분동안 화면 뚫어지게 쳐다보고 있을거 아니니까
/// 진짜 미친사람 아니고서야..일단 킵
/// -3.상점 일일재화(젬 획득)
/// -4.상점 아이템 상자 구매(젬 소모)
/// -5.상점 일일재화(골드 획득)
/// -6.장비 레벨업시(골드 소모)
/// +재화 축약표현은 K,M 까지만,
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

    /// <summary>
    /// 메인메뉴 들어올 때 호출
    /// </summary>
    private void OnEnable()
    {
        RefreshHUD();
    }

    /// <summary>
    /// 외부에서 호출 할 HUD 갱신 함수
    /// </summary>
    public void RefreshHUD()
    {
        haveGoldText.text = ChangeTextUnit(playerInfo.gold);
        haveGemText.text = ChangeTextUnit(playerInfo.gem);
        haveStaminaText.text = ChangeTextUnit(playerInfo.stamina);
        maxStaminaText.text = ChangeTextUnit(playerInfo.maxStamina);
    }

    //텍스트 중에 스태미나는 그렇다 쳐도, 골드랑 젬은 축약 표현 들어가야지

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
    //소수점 표현
    private string DecimalPoint(float value)
    {
        //기본 문자열 형식은 소수점 표현된 상태로
        string text = value.ToString("0.0");
        //소수점이 정확하게.0이면
        if (text.EndsWith(".0"))
        {
            //뒤에 2자리 지워버리기
            text = text.Substring(0, text.Length - 2);
        }
        return text;
    }
}
