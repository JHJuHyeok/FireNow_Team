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
    //비활성화할 패널
    [Header("비활성화 할 루트패널")]
    [SerializeField] private GameObject canvasGachaBox;

    //내가 표시할 결과 시각적 정보들
    [Header("가챠 결과 UI")]
    //등급 테두리 이미지
    [SerializeField] private Image gradeBorderImage;
    //아이템 이미지
    [SerializeField] private Image itemImage;
    //아이템이름
    [SerializeField] private TextMeshProUGUI itemNameText;
    //등급 이름
    [SerializeField] private TextMeshProUGUI gradeNameText;

    [Header("등급 이미지 맵핑 SO")]
    [SerializeField] private ItemGradeDB itemGradeDB;

    //캔버스 비활성화 할 수 있는 시간
    [Header("캔버스 비활성 가능 시간")]
    [SerializeField] private float canvasCloseDelay = 1.0f;

    //연출 시작된 시간 저장용 
    private float _showTime;

    //연출 캔버스가 켜진 후 가챠 결과기반 UI갱신
    public void RefreshResultUI(GachaResult result)
    {
        //닫기 대기시간 계산용 --여기서 진짜 개빡치는줄 왜? -스케일타임으로 건들면 안됨
        //스케일타임으로 영향x 언스케일드로 실제시간 반영
        _showTime = Time.unscaledTime;

        //아이템 이미지 세팅
        itemImage.sprite = result.ItemIcon;
        //아이템 이름 세팅
        itemNameText.text = result.ItemName;
        //등급 이름 세팅
        gradeNameText.text = result.grade.ToString();
        //등급 이미지 세팅
        gradeBorderImage.sprite = itemGradeDB.GetBorder(result.grade);
    }

    /// <summary>
    /// 탭하여 닫기용 이벤트 핸들러
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnPointerClick(PointerEventData eventData)
    {
        //지난 시간 계산
        float elapsed = Time.unscaledTime - _showTime;
        //아직 닫기 가능시간 아니면 무시
        if (elapsed < canvasCloseDelay) return;
        //탭하여 닫기
        canvasGachaBox.SetActive(false);
    }
}
