using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//각 맵 카드에 붙일 스크립트
//아직 데이터 관련 정보가 없으니까 인스펙터에서 직접 값 넣을 수 있게
//나중에 데이터 형식에 맞게 수정해야할 필요 있음
public class MapCard : MonoBehaviour
{
    [Header("맵 카드 기본 정보")]
    //맵ID
    [SerializeField] private int mapID;
    //맵 이름
    [SerializeField] private string mapName;
    //맵 설명
    [SerializeField] private string mapDescript;
    //맵 이미지
    [SerializeField] private Image mapImage;

    //실제로 보여질 이미지++
    [SerializeField] private Sprite mapSprite;

    [Header("해금 상태 관련")]
    //해금 여부
    [SerializeField] private bool isUnLock;

    //해당 맵 카드의 RectTransform 저장 용도 
    private RectTransform _rectTransform;

    //밖에서 읽기 가능한 프로퍼티 만들어야 되고
    //=====기본 정보=====
    public int MapID { get { return mapID; } }
    public string MapName { get { return mapName; } }
    public string MapDescript { get { return mapDescript; } }

    //++일단 테스트
    public Sprite MapSprite { get { return mapSprite; } }

    //=====해금상태관련=====
    public bool IsUnLock { get { return isUnLock; } }

    //=====RectTransform=====
    public RectTransform RectTransform { get { return _rectTransform; } }

    private void Awake()
    {
        //해당 오브젝트의 렉트트랜스폼 겟 컴포넌트
        _rectTransform = GetComponent<RectTransform>();
        //여기서 갱신 함수 호출
        RefreshMapCard();
    }

    /// <summary>
    /// 현재 해금 상태에 맞게 카드 이미지를 갱신해줄 함수
    /// </summary>
    private void RefreshMapCard()
    {
        //해금 상태에 해금된 이미지
        if (isUnLock)
        {
            mapImage.color = Color.white;
        }
        //미해금 상태에 미해금 이미지 -같은 이미지를 쓰되, 색상만 변경
        else
        {
            mapImage.color = Color.gray;
        }
    }

    /// <summary>
    /// 외부에서 맵을 해금할때 호출할 함수
    /// </summary>
    /// <param name="value"></param>
    public void SetUnlocked(bool value)
    {
        //해금 상태 변경
        isUnLock = value;
        //바뀐 상태에 따라 맵카드 색상갱신
        RefreshMapCard();
    }
}
