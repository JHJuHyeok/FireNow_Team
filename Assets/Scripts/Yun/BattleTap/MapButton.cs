using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 배틀탭내의 맵 버튼 클릭시, 맵 선택 패널 활성화
/// </summary>
public class MapButton : MonoBehaviour
{
    [Header("MapSelect 패널")]
    [SerializeField] private GameObject MapSelectPanel;

    public void OnShowMapPanel()
    {
        MapSelectPanel.SetActive(true);
    }
}
