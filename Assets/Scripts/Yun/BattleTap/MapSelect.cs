using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 맵선택 패널에서 맵 선택 기능 전반 담당
/// 맵 카드 스냅, 포커싱, UI표시(맵 이름,맵 설명), 선택 버튼(해금상태), 이용불가 텍스트(미해금상태)
/// </summary>
public class MapSelect : MonoBehaviour
{
    [Header("맵 선택 패널")]
    [SerializeField] private GameObject mapSelect;
    
    [Header("스크롤 뷰 관련")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform viewPort;
    [SerializeField] private RectTransform content;
    
    [Header("포커싱된 맵의 이름&설명")]
    [SerializeField] private TextMeshProUGUI mapName;
    [SerializeField] private TextMeshProUGUI mapDescript;
    
    [Header("선택 버튼 or 이용불가 텍스트")]
    [SerializeField] private Button selectButton;
    [SerializeField] private TextMeshProUGUI lockedText;
    
    [Header("뒤로가기 버튼")]
    [SerializeField] private Button returnButton;
    [Header("배틀탭 맵 버튼")]
    [SerializeField] private Button missionSelectButton;
    
    [Header("배틀탭 맵 선택 버튼 이미지")]
    [SerializeField] private Image missionSelect;
    [Header("배틀탭 맵 변경할 이름칸")]
    [SerializeField] private TextMeshProUGUI missionText;
    
    //컨텐트 아래에 속해있는 맵 카드 리스트-읽기전용
    private readonly List<MapCard> mapCards = new List<MapCard>();

    //현재 포커싱 된 맵 저장할 변수
    private MapCard focusedCard;

    //=====스냅 기능 관련=====

    [Header("해당 속도 이하면 스냅 시작")]
    [SerializeField] private float startSnapSpeed = 1.0f;

    [Header("스냅 속도")]
    [SerializeField] private float snapSpeed = 10.0f;
    
    private bool isSnapping = false;

    private void Awake()
    {
        mapSelect.SetActive(false);
        
        GetMapCards();
        
        selectButton.onClick.AddListener(SelectFocusedCard);
        returnButton.onClick.AddListener(CloseMapSelect);
        missionSelectButton.onClick.AddListener(OpenMapSelect);
        
        UpdateFocusUI();
    }

    private void Update()
    {
        if (mapSelect.activeSelf == false) return;
        
        //현재 스크롤뷰에서 가까운 카드 지정
        MapCard newFocus = GetCloserCard();
        
        //포커스 카드가 변하면 UI갱신
        if (newFocus != focusedCard)
        {
            focusedCard = newFocus;
            RefreshFocusUI(focusedCard);
        }

        //==스냅관련==

        if (!isSnapping && scrollRect.velocity.magnitude <= startSnapSpeed)
        {
            isSnapping = true;
        }

        if (isSnapping)
        {
            SnapToFocusedCard();
        }
    }

    /// <summary>
    /// 배틀탭=>맵 버튼 클릭시 맵선택 패널 활성화
    /// </summary>
    public void OpenMapSelect()
    {
        SoundManager.Instance.PlaySound("ButtonClick");
        mapSelect.SetActive(true);

        //패널 열릴때 포커스/표시 새로 계산
        UpdateFocusUI();
    }

    /// <summary>
    /// 맵 선택 패널의 뒤로가기 버튼 클릭시 패널 비활성화
    /// </summary>
    public void CloseMapSelect()
    {
        SoundManager.Instance.PlaySound("ButtonClick");
        mapSelect.SetActive(false);
    }

    /// <summary>
    /// 카드목록, 포커싱된 카드, UI 갱신
    /// </summary>
    private void UpdateFocusUI()
    {
        //카드 목록 불러오기
        GetMapCards();
        //포커싱 카드 계산
        focusedCard = GetCloserCard();
        //UI갱신
        RefreshFocusUI(focusedCard);
    }

    /// <summary>
    /// 컨텐트내 맵 카드들 리스트에 저장
    /// </summary>
    private void GetMapCards()
    {
        mapCards.Clear();
        content.GetComponentsInChildren<MapCard>(true, mapCards);
    }

    /// <summary>
    /// 포커싱 관련-뷰포인트 가운데 기준으로 가장 가까운 맵 카드 판단
    /// </summary>
    /// <returns></returns>
    private MapCard GetCloserCard()
    {
        //뷰포트의 가운데 지점의 좌표-기준점
        Vector3 viewPortCenter = viewPort.TransformPoint(viewPort.rect.center);

        //초기 거리 설정값 최대치로 설정 
        float closerDis = float.MaxValue;

        //가까운 카드도 일단 널 처리, 순회부분에서 판단
        MapCard closerCard = null;

        //MapCard 리스트를 순회하면서 거리체크
        int i = 0;
        while (i < mapCards.Count)
        {
            MapCard card = mapCards[i];

            //여기서 각 카드의 좌표 중앙기준점을 잡아주고
            RectTransform cardRect = card.RectTransform;
            Vector3 cardCenter = cardRect.TransformPoint(cardRect.rect.center);

            //카드의 중앙 기준점과 뷰포트의 중앙기준점 비교
            float distance = Mathf.Abs(cardCenter.x - viewPortCenter.x);
            //맥스 거리 보다 계산거리가 작으면 가까운것으로 간주
            if (distance < closerDis)
            {
                closerDis = distance;
                closerCard = card;
            }
            i++;
        }
        return closerCard;
    }

    /// <summary>
    /// 포커싱된 맵카드의 정보 표시
    /// </summary>
    /// <param name="card"></param>
    private void RefreshFocusUI(MapCard card)
    {
        //포커싱된 카드가 없으면 UI표시x 
        if (card == null)
        {
            mapName.text = string.Empty;
            mapDescript.text = string.Empty;
            selectButton.gameObject.SetActive(false);
            lockedText.gameObject.SetActive(false);
            return;
        }

        mapName.text = card.MapName;
        mapDescript.text = card.MapDescript;
        
        //해금조건 토글
        bool canSelect = card.IsUnLock;
        
        selectButton.gameObject.SetActive(canSelect);
        lockedText.gameObject.SetActive(!canSelect);
    }

    /// <summary>
    /// 포커싱된 맵카드 클릭시, 패널 비활성화, 맵버튼 스프라이트 변경
    /// </summary>
    private void SelectFocusedCard()
    {
        missionSelect.sprite = focusedCard.MapSprite;
        missionText.text = focusedCard.MapName;
        CloseMapSelect();
    }

    /// <summary>
    /// 포커싱된 맵 카드 스냅로직
    /// </summary>
    private void SnapToFocusedCard()
    {
        //뷰포트의 가운데 지점의 좌표-기준점
        Vector3 viewPortCenter = viewPort.TransformPoint(viewPort.rect.center);
        
        //포커싱된 카드 기반 가운데 지점의 좌표 
        RectTransform cardRect = focusedCard.RectTransform;
        Vector3 cardCenter = cardRect.TransformPoint(cardRect.rect.center);
        
        //거리계산
        float distance = viewPortCenter.x - cardCenter.x;

        //현재 컨텐트의 위치
        Vector2 curPos = content.anchoredPosition;
        
        //목표 위치
        Vector2 targetPos = new Vector2(curPos.x + distance, curPos.y);
        
        //현재 위치->목표위치로 이동
        content.anchoredPosition = Vector2.Lerp(curPos, targetPos, Time.deltaTime * snapSpeed);
        
        //가까워지면 스냅종료
        if (Mathf.Abs(distance) < 0.5f)
        {
            content.anchoredPosition = targetPos;
            isSnapping = false;
        }
    }
}
