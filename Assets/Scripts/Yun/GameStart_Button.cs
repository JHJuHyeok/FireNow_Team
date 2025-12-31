using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임시작 버튼 누를시, 보유 스태미나, 필요 스태미나 검증,
/// 시작 성공시, HUD 갱신, 씬로더 호출
/// </summary>
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
        if (_isStarting) return;
        _isStarting = true;
        
        //버튼 중복방지
        startButton.interactable = false;
        
        //==스태미나 검증 부분==
        int cost = startEnergySetting.selectedCost;
        int curStamina = playerInfo.stamina;
        if (curStamina < cost)
        {
            UIToast.ShowText("스태미나가 부족합니다!");
            startButton.interactable = true;
            return;
        }
        //스태미나 소모 및, 사용 시점 저장
        playerInfo.stamina = curStamina - cost;
        playerInfo.lastStaminaTime = DateTime.UtcNow.Ticks;

        SoundManager.Instance.PlaySound("ButtonClick");
        
        SaveManager.Instance.Save();
        
        hud.RefreshHUD(playerInfo);
        
        SoundManager.Instance.StopLoopSound("MainMenu_BGM");
        
        SceneLoader.Instance.LoadSceneWithFx(sceneName);
    }
}
