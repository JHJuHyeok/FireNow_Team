using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BGOverlay : MonoBehaviour
{
    // 배경 오버레이
    [SerializeField] private RectTransform overlay;

    private float topY;             // 화면 최상단 Y값
    private float bottomY;          // 화면 최하단 Y값

    private float overlayHeight;    // Viewport 기준 최대 높이

    /// <summary>
    /// 오버레이 변경
    /// </summary>
    /// <param name="slots"></param>
    /// <param name="slotIndex"></param>
    public void SyncOverlay(EvolSlotButton[] slots, int slotIndex)
    {
        float progress = GetProgress(slots, slotIndex);
        float targetHeight = overlayHeight * progress;

        // 트윈 충돌 방지
        overlay.DOKill();
        // 0.6초 동안 오버레이 사이즈 변경
        overlay.DOSizeDelta(new Vector2(overlay.sizeDelta.x, targetHeight), 0.6f)
            .SetEase(Ease.OutCubic);
    }

    /// <summary>
    /// 어디까지 오버레이를 진행하는지 산정
    /// </summary>
    /// <param name="slots"> 배치된 슬롯 배열 </param>
    /// <param name="slotIndex"> 오버레이가 진행된 슬롯의 인덱스 값 </param>
    /// <returns> 전체 화면과 오버레이의 비율 </returns>
    public float GetProgress(EvolSlotButton[] slots, int slotIndex)
    {
        float slotY;

        if (slotIndex != slots.Length - 1)
            slotY = Mathf.Abs(slots[slotIndex].transform.position.y);
        else
            slotY = topY;

        float lastY = Mathf.Abs(bottomY);

        return Mathf.Clamp01(slotY / lastY);
    }

    /// <summary>
    /// 게임 시작 시 슬롯 배열에 따라 최소 높이, 최대 높이 산정
    /// </summary>
    /// <param name="slots"> 배치된 슬롯 배열 </param>
    public void CacheEvolvesBounds(EvolSlotButton[] slots)
    {
        topY = slots[slots.Length - 1].transform.position.y;
        bottomY = slots[0].transform.position.y;
    }
}
