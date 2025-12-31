using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 맵카드 각각의 정보를 담을 클래스
/// </summary>
public class MapCard : MonoBehaviour
{
    [Header("맵 카드 기본 정보")]
    [SerializeField] private string mapId;
    [SerializeField] private string mapName;
    [SerializeField] private string mapDescript;
    [SerializeField] private Image mapImage;

    //맵 버튼의 변경될 스프라이트
    [SerializeField] private Sprite mapSprite;

    [Header("해금 상태 관련")]
    [SerializeField] private bool isUnLock;

    //해당 맵 카드의 RectTransform 저장 용도 
    private RectTransform _rectTransform;

    //=====기본 정보=====
    public string MapID { get { return mapId; } }
    public string MapName { get { return mapName; } }
    public string MapDescript { get { return mapDescript; } }
    public Sprite MapSprite { get { return mapSprite; } }

    //=====해금상태관련=====
    public bool IsUnLock { get { return isUnLock; } }

    //=====RectTransform=====
    public RectTransform RectTransform { get { return _rectTransform; } }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        RefreshMapCard();
    }

    /// <summary>
    /// 현재 해금 상태에 맞게 카드 이미지를 갱신
    /// 해금=원본색 미해금=회색
    /// </summary>
    private void RefreshMapCard()
    {
        if (isUnLock)
        {
            mapImage.color = Color.white;
        }
        else
        {
            mapImage.color = Color.gray;
        }
    }

    /// <summary>
    /// 외부에서 맵을 해금할때 호출
    /// </summary>
    /// <param name="value"></param>
    public void SetUnlocked(bool value)
    {
        //해금 상태 변경
        isUnLock = value;
        RefreshMapCard();
    }
}
