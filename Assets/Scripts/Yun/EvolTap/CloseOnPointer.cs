using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 스크롤뷰가 이벤트를 선점해서 이벤트핸들러,레이캐스트 필터 조합으로 우회
/// 특정영역 외의 공간 클릭시 지정패널 비활성화
/// ++장비탭에서도 쓸수 있게 변경
/// </summary>
public class CloseOnPointer : MonoBehaviour, IPointerDownHandler, ICanvasRaycastFilter
{
    [Header("인포패널(닫힐 대상)")]
    [SerializeField] private GameObject infoPanel;

    [Header("닫기 입력 받을 이벤트패널")]
    [SerializeField] private GameObject closePointPanel;

    [Header("클릭을 허용할 버튼/위치")]
    [SerializeField] private RectTransform buttonRect;

    /// <summary>
    /// 버튼영역 안이면 false, 밖이면 true, false일때 버튼클릭가능
    /// </summary>
    /// <param name="screenPoint"></param>
    /// <param name="eventCamera"></param>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        bool isInsideButton = RectTransformUtility.RectangleContainsScreenPoint(buttonRect, screenPoint, eventCamera);
        //버튼 안에서 레이캐스트 받지 않게
        return !isInsideButton;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySound("ClosePopup");
        infoPanel.SetActive(false);
        closePointPanel.SetActive(false);
    }
}
