using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 옵션 패널 안의 토글 항목 1개만을 담당
/// 적용 요소-효과음,배경음,진동,조이스틱,이펙트 약화 
/// 현재 on/off 상태에 따라 아이콘과 토글이미지 변경
/// 
/// 옵션 저장값은 세이브데이터에 영구저장(set)
/// 토글시 즉시반영을 위한 값은 플레이어 인포에서(get)
/// </summary>
public class OptionToggleControl : MonoBehaviour
{
    /// <summary>
    /// 토글 타입분류
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
    
    [Header("옵션 아이콘 이미지")]
    [SerializeField] private Image iconImage;
    
    [Header("옵션 토글 이미지")]
    [SerializeField] private Image toggleImage;

    [Header("옵션 아이콘 on/off 이미지")]
    [SerializeField] private Sprite onIconImage;
    [SerializeField] private Sprite offIconImage;

    [Header("옵션 토글 on/off 이미지")]
    [SerializeField] private Sprite onToggleImage;
    [SerializeField] private Sprite offToggleImage;
    
    private void Awake()
    {
        toggleButton.onClick.AddListener(Toggle);

        RefreshFromPlayerInfo(); 
    }

    /// <summary>
    /// 옵션토글 클릭시, 이전 상태의 반전상태로 변경
    /// </summary>
    private void Toggle()
    {
        SoundManager.Instance.PlaySound("ButtonClick");

        //현재 옵션 상태 기준 현재값과 다음값 변수 저장
        bool curValue = GetOptionToggleValue();
        bool nextValue = !curValue;

        //토글시 이전값의 반전값으로
        SetOptionToggleValue(nextValue);
        //토글상태 저장
        SaveManager.Instance.Save();
        //값변경 사실 이벤트 발생 기점
        OptionEvent.OptionChanged(toggleOptionType, nextValue);
        //nextValue 기준으로 UI갱신
        RefreshToggleUI(nextValue);
    }

    /// <summary>
    /// 토글직후 상태에 맞게 UI갱신
    /// </summary>
    /// <param name="isOn"></param>
    private void RefreshToggleUI(bool isOn)
    {
        //토글 이미지
        toggleImage.sprite = isOn ? onToggleImage : offToggleImage;
        //아이콘 이미지
        iconImage.sprite = isOn ? onIconImage : offIconImage;

    }

    /// <summary>
    /// playerInfo의 현재 값으로 UI갱신
    /// </summary>
    private void RefreshFromPlayerInfo()
    {
        bool value = GetOptionToggleValue();
        RefreshToggleUI(value);
    }

    /// <summary>
    /// 토글시 즉시반영을 위한 값->플레이어 인포에서(get)
    /// </summary>
    /// <returns></returns>
    public bool GetOptionToggleValue()
    {
        //플레이어 인포 참조(세이브 기준)
        PlayerInfoSO playerInfo = SaveManager.Instance.playerInfo;
        //각 타입마다 해당하는 상태 반환
        if (toggleOptionType == ToggleOptionType.SFX) return playerInfo.optionSfxOn;
        if (toggleOptionType == ToggleOptionType.BGM) return playerInfo.optionBgmOn;
        if (toggleOptionType == ToggleOptionType.Vibrate) return playerInfo.optionVibrateOn;
        if (toggleOptionType == ToggleOptionType.FxWeak) return playerInfo.optionFxWeakOn;
        if (toggleOptionType == ToggleOptionType.JoyStick) return playerInfo.optionJoyStickOn;
        return true;
    }

    /// <summary>
    /// 옵션 저장값-> 세이브데이터(set)
    /// </summary>
    /// <param name="value"></param>
    public void SetOptionToggleValue(bool value)
    {
        //플레이어 인포 참조(세이브 기준)
        PlayerInfoSO playerInfo = SaveManager.Instance.playerInfo;
        //각 타입마다 해당 value 값 적용
        if (toggleOptionType == ToggleOptionType.SFX) playerInfo.optionSfxOn = value;
        if (toggleOptionType == ToggleOptionType.BGM) playerInfo.optionBgmOn = value;
        if (toggleOptionType == ToggleOptionType.Vibrate) playerInfo.optionVibrateOn = value;
        if (toggleOptionType == ToggleOptionType.FxWeak) playerInfo.optionFxWeakOn = value;
        if (toggleOptionType == ToggleOptionType.JoyStick) playerInfo.optionJoyStickOn = value;
    }
}
