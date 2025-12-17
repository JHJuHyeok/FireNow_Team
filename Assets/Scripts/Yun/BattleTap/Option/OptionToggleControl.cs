using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 옵션 패널 안의 토글 항목 1개만을 담당.-각 버튼오브젝트 루트에 넣을것
/// 지금 염두한 것 효과음,배경음,진동,조이스틱,이펙트 약화 - 토글만 변경  이펙트 약화는 변하는 이미지 없음
/// 현재 on/off 상태에 따라 아이콘과 토글이미지 변경
/// 
/// 추후 기능관련 제어는 스위치부분에 추가할 것
/// </summary>
public class OptionToggleControl : MonoBehaviour
{
    /// <summary>
    /// 토글이 어떤 옵션인지 구분하기 위한 타입분류
    /// 추후 실제 기능 연결짓기 위한 틀이라고 생각해주십쇼
    /// </summary>
    public enum ToggleOptionType
    {
        SFX,
        BGM,
        Vibrate,
        FxWeak,
        JoyStick
    }

    [Header("어떤 옵션인지")]
    [SerializeField] private ToggleOptionType toggleOptionType;

    [Header("토글 버튼")]
    [SerializeField] private Button toggleButton;
    
    //기본 이미지
    [Header("옵션 아이콘 이미지")]
    [SerializeField] private Image iconImage;
    
    [Header("옵션 토글 이미지")]
    [SerializeField] private Image toggleImage;

    //on off 상태에 따라 변경될 이미지
    [Header("옵션 아이콘 on/off 이미지")]
    [SerializeField] private Sprite onIconImage;
    [SerializeField] private Sprite offIconImage;

    [Header("옵션 토글 on/off 이미지")]
    [SerializeField] private Sprite onToggleImage;
    [SerializeField] private Sprite offToggleImage;

    //=====토글 상태 저장 변수=====//에디터 기본값 일단 전부 on 상태로 시작
    [SerializeField] private bool isOnToggle = true;

    //어웨이크 에서 버튼 이벤트 등록
    private void Awake()
    {
        toggleButton.onClick.AddListener(Toggle);

        //시작시 에디터 초기값으로 토글 UI갱신
        RefreshToggleUI();
    }

    //버튼 클릭으로 토글 반전되는 함수
    private void Toggle()
    {
        isOnToggle = !isOnToggle;

        //UI갱신
        RefreshToggleUI();
        //나중에 실제 기능 연결할 자리
        ApplySystemToToggle();
    }

    //현재 상태에 맞게 UI갱신할 함수
    private void RefreshToggleUI()
    {
        //토글 온일때
        if (isOnToggle)
        {
            iconImage.sprite = onIconImage;
            toggleImage.sprite = onToggleImage;
        }
        //토글 오프일때
        if (!isOnToggle)
        {
            iconImage.sprite = offIconImage;
            toggleImage.sprite= offToggleImage;
        }
    }

    //토글 타입별 실제 기능을 토글에 적용할 함수
    private void ApplySystemToToggle()
    {
        switch (toggleOptionType)
        {
            case ToggleOptionType.SFX:
                //여기에 효과음 on/off 실제 적용
                break;
            case ToggleOptionType.BGM:
                //여기에 배겸음 on/off 실제 적용
                break;
            case ToggleOptionType.Vibrate:
                //여기에 진동 on/off 실제 적용
                break;
            case ToggleOptionType.FxWeak:
                //여기에 이펙트 약화 on/off 실제 적용
                break;
            case ToggleOptionType.JoyStick:
                //여기에 조이스틱 표시 on/off 실제 적용
                break;
        }
    }

    //토글의 on/off 상태를 외부에서 읽기 위한 함수
    public bool GetOptionToggleValue()
    {
        return isOnToggle;
    }

    //토글의 on/off 상태를 외부에서 강제로 변경해야 될 수도 있지않을까?
    public void SetOptionToggleValue(bool value)
    {
        isOnToggle = value;
        //변경된 값으로 토글UI갱신
        RefreshToggleUI();
    }

    //외부에서 토글 타입 읽기 위한 함수
    public ToggleOptionType GetOptionType()
    {
        return toggleOptionType;
    }
}
