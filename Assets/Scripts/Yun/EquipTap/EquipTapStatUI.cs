using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 플레이어(Equipstat+EvolStat) 합산스탯 표시 UI
/// 장비탭 내 상단에 시각적 갱신
/// </summary>
public class EquipTapStatUI : MonoBehaviour
{
    [Header("공격력 표시")]
    [SerializeField] private TextMeshProUGUI atkText;
    [Header("체력 표시")]
    [SerializeField] private TextMeshProUGUI hpText;

    private void OnEnable()
    {
        StartCoroutine(RefreshOnNextFrame());
    }

    /// <summary>
    /// 합산결과 값을 갱신
    /// 장비 장착, 해제, 진화 해금시 호출
    /// </summary>
    /// <param name="stat"></param>
    public void RefreshStatUI(BattleStat stat)
    {
        //결과가 소수점으로 생길수 있어서, 정수값으로 보여주기위해 FloorToInt
        atkText.text = Mathf.FloorToInt(stat.finalAttack).ToString();
        hpText.text = Mathf.FloorToInt(stat.finalMaxHP).ToString();
    }

    /// <summary>
    /// 계산결과 OnEnable로 바로 들고오려니까 선행처리 안되서 다 터짐
    /// 관련 선행처리 먼저 된 다음 실행하게 할 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator RefreshOnNextFrame()
    {
        yield return null;

        //관련 시스템 선행 준비됐는지 체크
        if(EquipControl.Instance == null) yield break;
        if (EquipStatSystem.Instance == null) yield break;

        //최신 결과 계산(장비+진화 스탯 재계산)
        EquipStatSystem.Instance.RecalculateFromEquipSlots(EquipControl.Instance.equipSlots);
        BattleStat stat = EquipStatSystem.Instance.CurrentBattleStat;

        atkText.text = Mathf.FloorToInt(stat.finalAttack).ToString();
        hpText.text = Mathf.FloorToInt(stat.finalMaxHP).ToString();
    }
}
