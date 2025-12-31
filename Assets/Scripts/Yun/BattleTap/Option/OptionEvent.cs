using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 옵션변경을 각 시스템에 즉시 알리기 위한 이벤트버스
/// 신호보내는 함수는(타이밍은) OptionToggleControl에서 결정
/// </summary>
public static class OptionEvent
{
    //옵션 변경 이벤트
    public static event Action<OptionToggleControl.ToggleOptionType, bool> OnOptionChanged;

    //옵션 토글 컨트롤로 전달
    public static void OptionChanged(OptionToggleControl.ToggleOptionType type, bool value)
    {
        if (OnOptionChanged != null)
        {
            //변경사실 구독자에게 전달
            OnOptionChanged(type, value);
        }
    }
}

