using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class scrollAnim : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;

    [SerializeField] private float maxScale = 1.2f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float ScaleRange = 300f;
    [SerializeField] private float tweenTime = 0.15f;
    [SerializeField] private Ease ShowingEase = Ease.OutQuad;

    private RectTransform[] icons;

   

    private void Start()
    {
        int count = content.childCount;
        icons = new RectTransform[count];

        for(int i = 0; i < count; i++)
        {
            icons[i] = content.GetChild(i) as RectTransform;
        }

        scrollRect.onValueChanged.AddListener(listener => OnscrollValueChanged());
        OnscrollValueChanged();
    }

    public  void OnscrollValueChanged()
    {
        Vector3 viewportCenterWorld = viewport.TransformPoint(viewport.rect.center);
        Vector3 viewportCenterLocal = content.InverseTransformPoint(viewportCenterWorld);
        float centerX = viewportCenterLocal.x;

        foreach (RectTransform icon in icons)
        {
            float distance = Mathf.Abs(centerX - icon.localPosition.x);
            float t = Mathf.Clamp01(distance / ScaleRange);
            float targetScale = Mathf.Lerp(maxScale, minScale, t);

            icon.DOKill();
            icon.DOScale(Vector3.one * targetScale, tweenTime)
                .SetEase(Ease.OutQuad);
        }


    }


}


