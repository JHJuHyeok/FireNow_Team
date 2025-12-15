using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//맵 선택화면에서 되돌아가기 버튼-패널 비활성화
public class ReturnButton : MonoBehaviour
{
    [Header("MapSelect 패널")]
    [SerializeField] private GameObject MapSelectPanel;

    public void OnHideMapPanel()
    {
        MapSelectPanel.SetActive(false);
    }
}
