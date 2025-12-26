using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 인포패널 닫기용으로 이벤트 핸들러 쓸건데,
/// 스크롤뷰가 이벤트 핸들러 다 잡아먹어서,
/// 이벤트 핸들러용 패널 따로 만들고, 디폴트는 비활성화,
/// 인포패널이 열려있을때만 이벤트 먹게끔 활성화 하고,
/// 그리고 이벤트 먹으면 인포패널 닫히고, 해당패널도 닫히는 식으로
/// -했다가 버튼도 안눌림. 버튼은 예외처리 할것
/// </summary>
public class CloseOnPointer : MonoBehaviour, IPointerDownHandler, ICanvasRaycastFilter
{
    [Header("진화 인포패널")]
    [SerializeField] private GameObject evolInfoPanel;

    [Header("닫기 입력 받을 이벤트패널")]
    [SerializeField] private GameObject closePointPanel;

    [Header("클릭을 허용할 버튼-위치")]
    [SerializeField] private RectTransform evolButtonRect;

    /// <summary>
    /// 버튼영역 안이면 false, 밖이면 true, false일때 버튼클릭가능
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="eventCamera"></param>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        bool isInsideButton = RectTransformUtility.RectangleContainsScreenPoint(evolButtonRect, screenPoint, eventCamera);
        //버튼 안에서 레이캐스트 받지 않게
        return !isInsideButton;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        evolInfoPanel.SetActive(false);
        closePointPanel.SetActive(false);
    }
}
