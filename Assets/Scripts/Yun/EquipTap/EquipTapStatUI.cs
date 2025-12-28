using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 단순 플레이어 기본스탯 표시 UI
/// 장비 스탯과 진화 스탯을 계산한 battleStat의 파이널 값을 가져옴
/// </summary>
public class EquipTapStatUI : MonoBehaviour
{
    [Header("공격력 표시")]
    [SerializeField] private TextMeshProUGUI atkText;
    [Header("체력 표시")]
    [SerializeField] private TextMeshProUGUI hpText;

    //장비 탭 열릴때도 계산된 배틀스탯 들고올것
    private void OnEnable()
    {
        StartCoroutine(RefreshOnNextFrame());
    }

    //장비 장착, 해제, 진화 해금시 호출할것
    public void RefreshStatUI(BattleStat stat)
    {
        //결과가 소수점으로 생길수 있어서, 정수값으로 보여주기위해 FloorToInt
        atkText.text = Mathf.FloorToInt(stat.finalAttack).ToString();
        hpText.text = Mathf.FloorToInt(stat.finalMaxHP).ToString();
    }

    //계산결과 OnEnable로 바로 들고오려니까 선행처리 안되서 다 터짐
    //관련 선행처리 먼저 된 다음 실행하게 할 코루틴
    private IEnumerator RefreshOnNextFrame()
    {
        //일단 한프레임 대기
        yield return null;

        //관련 시스템 선행 준비됐는지 체크
        if(EquipControl.Instance == null) yield break;
        if (EquipStatSystem.Instance == null) yield break;

        //최신 결과 계산(장비+진화 스탯 재계산)
        EquipStatSystem.Instance.RecalculateFromEquipSlots(EquipControl.Instance.equipSlots);
        BattleStat stat = EquipStatSystem.Instance.CurrentBattleStat;

        //그 후에 UI갱신
        atkText.text = Mathf.FloorToInt(stat.finalAttack).ToString();
        hpText.text = Mathf.FloorToInt(stat.finalMaxHP).ToString();
    }
}
