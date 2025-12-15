using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//단순 배틀탭 맵 버튼 누르면 맵 선택 패널 활성화
public class MapButton : MonoBehaviour
{
    [Header("MapSelect 패널")]
    [SerializeField] private GameObject MapSelectPanel;

    public void OnShowMapPanel()
    {
        MapSelectPanel.SetActive(true);
    }
}
