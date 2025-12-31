using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 각 탭에 대응하는 패널 연결,
/// 배열식 관리-> 해당 인덱스를 탭 버튼 온클릭으로 연결
/// </summary>
public class TabManager : MonoBehaviour
{
    [Header("탭에 들어갈 패널")]
    [SerializeField] private GameObject[] panels;

    private int _curIndex = 0;

    /// <summary>
    /// 탭 누를때 실제 전환 기능
    /// 클릭된(현재) 인덱스패널 제외, 다른 패널 비활성화
    /// </summary>
    /// <param name="index"></param>
    public void OpenTap(int index)
    {
        //모든 탭 비활성화
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == index);
        }
        SoundManager.Instance.PlaySound("ButtonClick");
        _curIndex = index;
    }
}
