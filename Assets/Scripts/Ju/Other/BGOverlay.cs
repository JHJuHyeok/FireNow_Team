using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BGOverlay : MonoBehaviour
{
    [SerializeField] private RectTransform bgColorLayer;
    [SerializeField] private RectTransform fgContent;
    [SerializeField] private RectTransform viewport;

    //private RectTransform slotRect;
    //private Tween fillTween;

    public void UpdateBackgroundColor(EvolSlotButton slot)
    {
        RectTransform slotRect = slot.GetComponent<RectTransform>();

        // 컨텐트 안에서 노드의 높이
        float slotY = slotRect.anchoredPosition.y;
        // 현재 스크롤로 인해 위로 밀린 양
        float scrollY = fgContent.anchoredPosition.y;
        // 화면 기준 진화 영역 높이
        float height = slotY + scrollY + 18600;

        height = Mathf.Clamp(0, height, viewport.rect.height);

        bgColorLayer.sizeDelta = new Vector2(bgColorLayer.sizeDelta.x, height);
        //bgColorLayer.DOsizeDelta(new Vector2(bgColorLayer.sizeDelta.x, height), 0.5f);

        Debug.Log($"slotY:{slotY}, scrollY:{scrollY}, height:{height}, viewportHeight:{viewport.rect.height}");
    }
}
