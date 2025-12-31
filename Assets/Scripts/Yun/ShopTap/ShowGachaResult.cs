using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 가챠 결과물을 직접 시각적으로 보여줄 부분
/// 탭하여 닫기 기능은 이벤트 핸들러로 구현
/// </summary>
public class ShowGachaResult : MonoBehaviour , IPointerClickHandler
{
    [Header("비활성화 할 루트패널")]
    [SerializeField] private GameObject canvasGachaBox;

    [Header("가챠 결과 UI")]
    [SerializeField] private Image gradeBorderImage;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI gradeNameText;

    [Header("등급 이미지 맵핑 SO")]
    [SerializeField] private ItemGradeDB itemGradeDB;

    [Header("캔버스 비활성 가능 시간")]
    [SerializeField] private float canvasCloseDelay = 1.0f;

    //연출 시작된 시간 저장용 
    private float _showTime;

    /// <summary>
    /// 연출 캔버스가 켜진 후 가챠 결과기반 UI갱신
    /// </summary>
    /// <param name="result"></param>
    public void RefreshResultUI(GachaResult result)
    {
        _showTime = Time.unscaledTime;

        //아이템 이미지, 텍스트 세팅
        itemImage.sprite = result.ItemIcon;
        itemNameText.text = result.ItemName;
        gradeNameText.text = result.grade.ToString();
        gradeBorderImage.sprite = itemGradeDB.GetBorder(result.grade);
    }

    /// <summary>
    /// 탭하여 닫기용 이벤트 핸들러
    /// 가챠연출이 끝난후에만 작동 
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnPointerClick(PointerEventData eventData)
    {
        float elapsed = Time.unscaledTime - _showTime;
        if (elapsed < canvasCloseDelay) return;
        SoundManager.Instance.PlaySound("ClosePopup");
        canvasGachaBox.SetActive(false);
    }
}
