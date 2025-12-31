using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 가챠 버튼(랜덤 아이템 구매 버튼)용
/// GachaLogic 연계
/// </summary>
public class ShopGachaButton : MonoBehaviour
{
    [Header("연출 담당 캔버스")]
    [SerializeField] private GameObject canvasGachaBox;
    
    [Header("플레이어 인포(장비리스트쪽)")]
    [SerializeField] private PlayerInfoSO playerInfo;
    
    [Header("가챠 테이블 SO")]
    [SerializeField] private GachaTableSO gachaTable;
    
    [Header("캔버스 UI갱신부분")]
    [SerializeField] private ShowGachaResult gachaUI;
    
    [Header("구매 버튼")]
    [SerializeField] private Button gachaButton;

    [Header("HUD 참조")]
    [SerializeField] private HUD hud;

    private void Awake()
    {
        gachaButton.onClick.AddListener(OnClickGacha);
    }

    /// <summary>
    /// 가챠 버튼 클릭시, 비용체크
    /// 보유 젬 차감, HUD갱신, 결과물 데이터를 런타임 데이터로 생성
    /// 생성된 런타임 데이터를 플레이어 장비목록에 추가
    /// </summary>
    private void OnClickGacha()
    {
        int cost = gachaTable.gemCost;
        
        if (playerInfo.gem < cost)
        {
            SoundManager.Instance.PlaySound("Alert");
            UIToast.ShowText("보유 젬이 부족합니다!");
            return;
        }

        playerInfo.gem -= cost;
        SoundManager.Instance.PlaySound("MixedEquip");

        hud.RefreshHUD(playerInfo);

        //==가챠 로직 부분==
        GachaResult gachaResult = GachaLogic.DrawOnce(gachaTable);
        EquipInfo equipInfo = new EquipInfo();
        //==가챠 데이터 정의 부분==
        equipInfo.equip = gachaResult.equipDataRuntime;
        equipInfo.grade = gachaResult.grade;
        equipInfo.level = 1;

        //해당 장비를 플레이어 데이터 장비 목록에 추가
        playerInfo.equips.Add(equipInfo);

        //연출캔버스 활성화
        canvasGachaBox.SetActive(true);

        //해당 결과 아이템정보 UI 갱신
        gachaUI.RefreshResultUI(gachaResult);
        
        //저장지점
        SaveManager.Instance.Save();
    }
}
