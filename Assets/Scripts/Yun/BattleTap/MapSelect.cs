using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//배틀탭 || 하위-맵선택 컨트롤 오브젝트>에 컴포넌트로 적용 -관련 패널 (배틀탭,맵 선택)
//필요 기능- 스냅, 포커싱, UI표시(맵 이름,맵 설명), 선택 버튼(해금상태), 이용불가 텍스트(미해금상태)
public class MapSelect : MonoBehaviour
{
    //맵 선택 패널 (on/off용)
    [Header("맵 선택 패널")]
    [SerializeField] private GameObject mapSelect;
    //스크롤뷰 관련 (스크롤렉트, 뷰포트, 컨텐트)
    [Header("스크롤 뷰 관련")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform viewPort;
    [SerializeField] private RectTransform content;
    //포커싱 관련(포커싱된 맵의 이름,설명)
    [Header("포커싱된 맵의 이름&설명")]
    [SerializeField] private TextMeshProUGUI mapName;
    [SerializeField] private TextMeshProUGUI mapDescript;
    //선택 버튼 or 이용불가 텍스트
    [Header("선택 버튼 or 이용불가 텍스트")]
    [SerializeField] private Button selectButton;
    [SerializeField] private TextMeshProUGUI lockedText;
    //버튼 관련(뒤로가기 버튼, 배틀탭의 맵버튼)
    [Header("뒤로가기 버튼")]
    [SerializeField] private Button returnButton;
    [Header("배틀탭 맵 버튼")]
    [SerializeField] private Button missionSelectButton;
    //배틀탭에서 보여지는 이미지 , 맵 이름, 클리어 여부
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
    //스냅 속도
    [Header("스냅 속도")]
    [SerializeField] private float snapSpeed = 10.0f;
    //스냅 중인지 플래그
    private bool isSnapping = false;

    private void Awake()
    {
        //맵선택 패널 비활성화로 시작
        mapSelect.SetActive(false);
        //카드 목록 한번 수집하고 시작해야되고,
        GetMapCards();
        //선택 버튼 이벤트 등록
        selectButton.onClick.AddListener(SelectFocusedCard);
        //뒤로가기 버튼 이벤트 등록
        returnButton.onClick.AddListener(CloseMapSelect);
        //배틀탭-맵 버튼 이벤트 등록
        missionSelectButton.onClick.AddListener(OpenMapSelect);
        //종합갱신함수 호출
        UpdateFocusUI();
    }

    private void Update()
    {
        //맵 선택 패널이 꺼져있으면 아무것도 하지말고
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
    /// 맵 선택 패널 열기(메인메뉴에서) 일단 단순 활성화 //임시로 만들어둔 패널 관련cs 지울것
    /// </summary>
    public void OpenMapSelect()
    {
        SoundManager.Instance.PlaySound("ButtonClick");
        mapSelect.SetActive(true);

        //패널 열릴때 포커스/표시 새로 계산
        UpdateFocusUI();
    }

    /// <summary>
    /// 맵 선택 패널 닫기(맵선택 패널에서) 일단 단순 비활성화
    /// </summary>
    public void CloseMapSelect()
    {
        SoundManager.Instance.PlaySound("ButtonClick");
        mapSelect.SetActive(false);
    }

    /// <summary>
    /// 카드목록, 포커싱된 카드, UI 갱신 함수 
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
    /// 컨텐트 아래 맵 카드 전부 찾아서 리스트에 저장할 함수
    /// </summary>
    private void GetMapCards()
    {
        mapCards.Clear();
        content.GetComponentsInChildren<MapCard>(true, mapCards);
    }

    /// <summary>
    /// 포커싱 관련 뷰포인트 가운데 기준 가장 가까운 맵 카드 찾을 함수
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
    /// 포커싱된 맵카드 기반의 정보 표시 함수
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

        //이름 설명 표시
        mapName.text = card.MapName;
        mapDescript.text = card.MapDescript;
        //해금조건 토글
        bool canSelect = card.IsUnLock;
        //선택버튼 이용불가 텍스트 표시 
        selectButton.gameObject.SetActive(canSelect);
        lockedText.gameObject.SetActive(!canSelect);
    }

    /// <summary>
    /// 선택 버튼 눌렀을때 함수
    /// </summary>
    private void SelectFocusedCard()
    {
        //배틀 맵 선택 버튼 이미지를 포커싱된 맵 이미지로 변경 
        missionSelect.sprite = focusedCard.MapSprite;
        //배틀 맵 미션이름 포커싱된 맵 이름으로 변경
        missionText.text = focusedCard.MapName;
        //패널닫기
        CloseMapSelect();
    }

    /// <summary>
    /// 스냅기능함수
    /// </summary>
    private void SnapToFocusedCard()
    {
        //뷰포트의 가운데 지점의 좌표-기준점
        Vector3 viewPortCenter = viewPort.TransformPoint(viewPort.rect.center);
        //포커싱된 카드 기반 가운데 지점의 좌표 
        RectTransform cardRect = focusedCard.RectTransform;
        Vector3 cardCenter = cardRect.TransformPoint(cardRect.rect.center);
        //둘의 거리계산
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
