using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//게임시작 버튼 누르면 동작할 기능
//-스태미나 체크, 차감,저장, <HUD 갱신은(보류)>
//-연출 포함한 씬로더를 호출
//-씬로더 온클릭에 지정
public class GameStart_Button : MonoBehaviour
{
    [Header("HUD 참조")]
    [SerializeField] private HUD hud;

    [Header("플레이어 인포")]
    [SerializeField] private PlayerInfoSO playerInfo;

    [Header("전환할 씬 이름")]
    [SerializeField] private string sceneName;

    [Header("배수/스태미나 비용 제공")]
    [SerializeField] private StartEnergySetting startEnergySetting;

    [Header("중복클릭 방지(게임시작버튼)")]
    [SerializeField] private Button startButton;
    
    //게임시작 로직 시작됐다는 플래그
    private bool _isStarting; 

    public void OnClickStart()
    {
        //게임 시작버튼 실행중이면 취소
        if (_isStarting) return;
        _isStarting = true;
        //버튼 비활성화를 여기서
        startButton.interactable = false;
        //필요한 스태미나 비용 계산
        int cost = startEnergySetting.selectedCost;
        //플레이어 인포 기반으로 저장된 스태미나 체크
        int curStamina = playerInfo.stamina;
        //비용 대비 보유 스태미나 부족한 경우-UI토스트 출력
        if (curStamina < cost)
        {
            UIToast.ShowText("스태미나가 부족합니다!");
            startButton.interactable = true;
            return;
        }
        //데이터에서 보유 스태미나 감소 뒤,
        playerInfo.stamina = curStamina - cost;
        //스태미나를 사용한 지금 이순간을 라스트 스태미나 타임으로
        playerInfo.lastStaminaTime = DateTime.UtcNow.Ticks;
        SoundManager.Instance.PlaySound("ButtonClick");
        //저장 한번 하고,
        SaveManager.Instance.Save();
        //HUD 갱신
        hud.RefreshHUD(playerInfo);
        SoundManager.Instance.StopLoopSound("MainMenu_BGM");
        //효과 포함한 씬로더 호출
        SceneLoader.Instance.LoadSceneWithFx(sceneName);
    }
}
