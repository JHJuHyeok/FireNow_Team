using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 레벨마커 프리팹용 스크립트
/// 매개변수 값에 맞게 레벨텍스트 변경,
/// 비활성화 활성화 상태에 따라 스프라이트 변경
/// </summary>
public class LevelMarker : MonoBehaviour
{
    [Header("레벨마커 디폴트")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("레벨별 변경될 스프라이트")]
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite activeSprite;

    /// <summary>
    /// 표시 레벨 텍스트 세팅
    /// </summary>
    public void SetLevel(int level)
    {
        levelText.text = level.ToString();
    }

    /// <summary>
    /// 레벨 기준 활성/비활성 스프라이트 교체함수
    /// </summary>
    public void SetActive(bool isActive)
    {
        icon.sprite = isActive ? activeSprite : defaultSprite;
    }
}
