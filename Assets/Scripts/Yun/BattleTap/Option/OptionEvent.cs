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
    //옵션 변경 이벤트 넣어주고 -구독자들이 구독할거
    public static event Action<OptionToggleControl.ToggleOptionType, bool> OnOptionChanged;

    //옵션 토글 컨트롤로 넘겨줄거
    public static void OptionChanged(OptionToggleControl.ToggleOptionType type, bool value)
    {
        if (OnOptionChanged != null)
        {
            //변경사실 전달
            OnOptionChanged(type, value);
        }
    }
}

/*
 외부에서 이벤트 구독할때 예시입니다!
사운드 매니저에서 쓴다면,
public class SoundManager : 모노비하이비어
{
    Private void OnEnabel() 활성화 때 구독
    {
        OptionEvent.OnOptionChanged += 옵션변경이벤트 함수 참조 -함수 자체를 참조하는거라 ()필요없슴다!
    }

    private void OnDisable() 비활성화 때 구독해제
    {
        OptionEvent.OnOptionChanged += 옵션변경이벤트 함수 참조 
    }

    //아마 사운드 매니저는 DDL 일거니까.. 활성화 비활성화 때 구독,구독해제 말고,
    //그냥 시작시 start 쪽에서 구독만 해놓으면 될거 같슴다 위에는 전투씬에도 넣어야해서 예시로 썼어요
    
    //옵션 변경 이벤트 처리할 함수 하나
    private void 옵션변경이벤트 함수(OptionToggleControl.ToggleOptionType type, bool value)
    {
        //사운드중 SFX라면,
        if(type == OptionToggleControl.ToggleOptionType.SFX)
        {
            여기에 SFX 켜졌을때/안켜졌을때 조건분기
            if(value == true)
            {
                //sfx켜는 로직
            }
            else
            {
                //sfx끄는 로직
            }
        }
        //사운드중 BGM 이라면.. 위랑 같은식으로
    }
}
대충 이런식으로? 

 */

