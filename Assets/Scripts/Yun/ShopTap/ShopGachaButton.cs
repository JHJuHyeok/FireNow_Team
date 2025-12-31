using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 가챠 버튼(랜덤 아이템 구매 버튼)용
/// </summary>
public class ShopGachaButton : MonoBehaviour
{
    //연출 캔버스
    [Header("연출 담당 캔버스")]
    [SerializeField] private GameObject canvasGachaBox;
    //플레이어 데이터
    [Header("플레이어 인포(장비리스트쪽)")]
    [SerializeField] private PlayerInfoSO playerInfo;
    //가챠 테이블 데이터
    [Header("가챠 테이블 SO")]
    [SerializeField] private GachaTableSO gachaTable;
    //UI 갱신부분 - 아이템 이미지 아이템 등급 테두리 이미지 UI 쪽 배치 아직임
    [Header("캔버스 UI갱신부분")]
    [SerializeField] private ShowGachaResult gachaUI;
    //구매버튼
    [Header("구매 버튼")]
    [SerializeField] private Button gachaButton;

    //HUD 갱신 참조
    [Header("HUD 참조")]
    [SerializeField] private HUD hud;

    //어웨이크에서 버튼 이벤트 등록
    private void Awake()
    {
        gachaButton.onClick.AddListener(OnClickGacha);
    }

    //실제 버튼이 눌려지면 행할 것들
    private void OnClickGacha()
    {

        //필요한 비용은?
        int cost = gachaTable.gemCost;
        //필요한 비용 대비 보유 비용이 부족하다면?-UI토스트로 출력
        if (playerInfo.gem < cost)
        {
            UIToast.ShowText("보유 젬이 부족합니다!");
            return;
        }

        //젬 차감하고, 가챠 돌리기
        playerInfo.gem -= cost;
        SoundManager.Instance.PlaySound("MixedEquip");
        //여기서 HUD 갱신
        hud.RefreshHUD(playerInfo);
        //가챠 돌려서 나온 결과물 데이터
        GachaResult gachaResult = GachaLogic.DrawOnce(gachaTable);
        //보유 장비 정보 생성하고
        EquipInfo equipInfo = new EquipInfo();
        //런타임 장비
        equipInfo.equip = gachaResult.equipDataRuntime;
        //등급도 저장해주고
        equipInfo.grade = gachaResult.grade;
        //레벨 1로 지급
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
