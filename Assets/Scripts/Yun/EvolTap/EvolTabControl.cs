using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

/// <summary>
/// 실질적인 진화탭 컨트롤타워
/// -컨텐트에 슬롯,커넥터 프리팹 자동생성
/// -슬롯 클릭시, 인포패널을 슬롯 위에 보이게,
/// -해금조건(중요)-플레이어 레벨기반 && 이전슬롯 해금여부
/// -비용은 ++새로 cost 값 데이터 생김
/// .>>evolveDatabase.json -정의 데이터
/// .>>evolveLevelConfig.json -비용과 3슬롯 config(evolveId,value)
/// -비용차감은 PlayerInfoSO.gold에서,
/// -스탯적용은 BaseStatSO에 적용
/// 이제 레벨단위로 해금(기존은 슬롯단위였음)
/// </summary>
public class EvolTabControl : MonoBehaviour
{
    [Header("스크롤뷰 컨텐트")]
    [SerializeField] private Transform content;
    [SerializeField] private ScrollRect scrollRect;

    [Header("사용 할 프리팹")]
    [SerializeField] private EvolSlotButton slotPrefab;
    [SerializeField] private GameObject connectorPrefab;
    [Header("레벨마커 프리팹")]
    [SerializeField] private LevelMarker markerPrefab;

    //마커 x좌표는 항상 0(중앙)
    [SerializeField] private float markerX = 0.0f;

    //위치 가져와야돼 버튼 위에 똑 하고 생길거니까(고정형 아님)
    [Header("진화 인포 패널 관련")]
    [SerializeField] private EvolInfoPanel infoPanel;
    [SerializeField] private RectTransform infoPanelRect;
    //얼마나 높이띄울지는 인스펙터에 조절 가능하게
    [SerializeField] private float infoPanelY = 10.0f;

    [Header("플레이어 데이터-재화,레벨 부분")]
    [SerializeField] private PlayerInfoSO playerInfoSO;

    [Header("플레이어 데이터-스탯부분")]
    [SerializeField] private BaseStatSO baseStatSO;

    //레벨당 슬롯 수(고정 3)
    private const int _slotPerLevel = 3;
    //기본 진화 단계(고정 60개)
    private const int _totalSlotCount = 60;
    //3슬롯 할당 레벨 구간
    private const int _maxLevel = _totalSlotCount / _slotPerLevel;

    //생성된 슬롯,커넥터,마커 저장용 배열
    private EvolSlotButton[] _evolSlots;
    private GameObject[] _connectors;
    private LevelMarker[] _levelMarkers;

    //인포패널에서 표시할, 선택된 슬롯 인덱스
    private int _selectedSlotIndex;

    //마커 커넥터 갱신 코루틴 중복방지
    private Coroutine _repositionCO;

    private void Awake()
    {
        //DB관련 초기화- 이부분 다 완성되고 나면 부트스트랩쪽으로 보내버리기
        EvolveLevelConfigDatabase.Initialize();
        EvolveDatabase.Initialize();

        //인포패널 레이아웃은 배치요소에서 제외
        LayoutElement layoutElement = infoPanel.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = infoPanel.gameObject.AddComponent<LayoutElement>();
        }
        layoutElement.ignoreLayout = true;

        //패널이 해금요청 보내게 컨트롤 연결
        infoPanel.BindControl(this);
        //초기상태는 숨긴 상태로
        infoPanel.Hide();
        //슬롯,커넥터,마커 생성
        CreateInContent();
        //UI갱신하고 시작
        RefreshAll();
    }

    /// <summary>
    /// 컨텐트에 슬롯, 커넥터 프리팹 자동 생성함수
    /// -슬롯은 60개
    /// -커넥터는 59개
    /// -마커는 20개(1레벨당 3슬롯)
    /// </summary>
    private void CreateInContent()
    {
        //기존거 있으면 제거(중복방지용)-안에 들어있는 패널은 예외
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Transform child = content.GetChild(i);
            if (child.gameObject == infoPanel.gameObject) continue;
            
            Destroy(child.gameObject);
        }

        //슬롯 배열 생성
        _evolSlots = new EvolSlotButton[_totalSlotCount];
        //슬롯 이어줄 커넥터 배열도 생성(생성될 슬롯 갯수-1)
        _connectors = new GameObject[_totalSlotCount - 1];
        //마커 배열 생성
        _levelMarkers = new LevelMarker[_maxLevel];

        //슬롯, 커넥터 생성부분
        for (int slotIndex = 0; slotIndex < _totalSlotCount; slotIndex++)
        {
            //슬롯생성
            EvolSlotButton slotInstance = Instantiate(slotPrefab, content);
            //슬롯에 인덱스/컨트롤 부여
            slotInstance.Initialize(slotIndex, this);
            //배열에 저장
            _evolSlots[slotIndex] = slotInstance;

            //커넥터 부분
            if (slotIndex < _totalSlotCount - 1)
            {
                GameObject connectorInstance = Instantiate(connectorPrefab, content);

                //레이아웃 그룹이 배치요소로 취급하지 않게
                LayoutElement connetorLayout = connectorInstance.GetComponent<LayoutElement>();
                if (connetorLayout == null)
                {
                    connetorLayout = connectorInstance.AddComponent<LayoutElement>();
                }
                connetorLayout.ignoreLayout = true;
                //배열저장
                _connectors[slotIndex] = connectorInstance;
            }
        }
        //마커 생성부분
        for (int level = 1; level <= _maxLevel; level++)
        {
            LevelMarker markerInstance = Instantiate(markerPrefab, content);
            //마커도 레이아웃 그룹 배치요소 제외
            LayoutElement markerLayout = markerInstance.GetComponent<LayoutElement>();
            if (markerLayout == null)
            {
                markerLayout = markerInstance.gameObject.AddComponent<LayoutElement>();
            }
            markerLayout.ignoreLayout = true;

            markerInstance.SetLevel(level);
            //배열저장
            _levelMarkers[level-1] = markerInstance;
        }
        //마커, 커넥터 갱신 코루틴 요청
        RequestRepositionCO();
    }

    //위치 기반 계산식 일단 주석처리

    /// <summary>
    /// 마커 위치 세팅 함수
    /// +-레벨마다 그니까 3단계 슬롯위치에 배치되어야 하고, x좌표는 중앙 0 고정이잖아
    /// 각레벨 데이터가 새로 생겼으니까 구차하게 따로 슬롯 위치 빼서 사용할 필요는 없을듯?
    /// 레벨 기준으로 움직이면 되니까  
    /// </summary>
    private void MarkerPosSetting()
    {
        for (int level = 1; level <= _maxLevel; level++)
        {
            //마커 배열 0베이스, -1 인덱스로 접근
            LevelMarker marker = _levelMarkers[level-1];
            if (marker == null) continue;
            //레벨N의 슬롯 index = 0베이스 니까,.(N * 3) - 1
            int markerSlotIndex = level * _slotPerLevel-1;

            //해당 슬롯의 RectTransform
            RectTransform slotRect = _evolSlots[markerSlotIndex].transform as RectTransform;
            //마커의 RectTransform
            RectTransform markerRect = marker.transform as RectTransform;

            //슬롯의 위치 기준으로 마커 위치 잡아야되고,
            //y기준은 슬롯 기준, x기준은 화면 정중앙 벡터 2 새로 줘서 위치 주는걸로
            Vector2 pos = slotRect.anchoredPosition;
            pos.x = markerX;
            markerRect.anchoredPosition = pos;
        }
    }

    /// <summary>
    /// 커넥터 위치세팅 함수--
    /// </summary>
    private void ConnectorsPosSetting()
    {
        //커넥터 전체를 순회하면서 위치/길이를 갱신
        for (int i = 0; i < _connectors.Length; i++)
        {
            //i번째 커넥터 지정
            GameObject connector = _connectors[i];

            //최소방어
            if (connector == null) continue;

            //해당 커넥터가 연결해야 할 슬롯들의 RectTransform계산
            //슬롯A(이전)
            RectTransform slotA = _evolSlots[i].transform as RectTransform;
            //슬롯B(다음)
            RectTransform slotB = _evolSlots[i + 1].transform as RectTransform;
            //커넥터의 RectTransform계산
            RectTransform connectorRect = connector.transform as RectTransform;

            //슬롯의 월드 좌표로 계산
            Vector3 a = slotA.position;
            Vector3 b = slotB.position;

            Vector3 middle;
            middle.x = (a.x + b.x) * 0.5f;
            middle.y = (a.y + b.y) * 0.5f;
            middle.z = connectorRect.position.z;
            //커넥터를 해당 위치로 이동
            connectorRect.position = middle;
        }
    }

    /// <summary>
    /// 슬롯 클릭시 슬롯 버튼쪽에서 이벤트 호출
    /// -slotIndex 기반으로 인포패널 위치 이동,내용 갱신
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="slotRect"></param>
    public void OnClickSlot(int slotIndex, RectTransform slotRect)
    {
        //현재 선택 슬롯 저장
        _selectedSlotIndex = slotIndex;
        //패널을 할상 가장 위로 보이도록 마지막 자식으로 보내버리기
        infoPanel.transform.SetAsLastSibling();
        //패널을 슬롯위로
        Vector2 pos = slotRect.anchoredPosition;
        //지정값만큼 위로 올려주고
        pos.y = pos.y + infoPanelY;
        //패널위치 적용
        infoPanelRect.anchoredPosition = pos;
        //패널 오픈
        OpenInfoPanel(slotIndex);
    }

    //slotIndex를 level과 indexInlevel로 변환함수
    private void ConvertSlotIndexToLevel(int slotIndex, out int level, out int indexInlevel)
    {
        int levelIndex = slotIndex / _slotPerLevel;
        level = levelIndex + 1;
        indexInlevel = slotIndex % _slotPerLevel;
    }

    /// <summary>
    /// slotIndex ->(level,indexInlevel)로 변환후에
    /// 레벨테이블 = evolveLevelConfigs의 config(evolveId,value)
    /// 노드 정의 테이블 = evolveDatabase
    /// 레벨테이블의 config랑 노드정의 테이블의 EvolveData를 조합해서 인포패널 내용표시
    /// 
    /// </summary>
    /// <param name="slotIndex"></param>
    private void OpenInfoPanel(int slotIndex)//**
    {
        //현재 클릭한 슬롯의 레벨 저장용
        int level;
        //해당 레벨안에서 몇번째 슬롯인지 저장용
        int indexInlevel;
        ConvertSlotIndexToLevel(slotIndex, out level, out indexInlevel);

        EvolveLevelConfig levelConfig = EvolveLevelConfigDatabase.GetLevel(level);

        //레벨 3개 슬롯 중 어떤 슬롯인지에 따라 config 선택
        //해당 레벨의 indexInlevel번째 설정 가져오기
        EvolveConfig config = levelConfig.configs[indexInlevel];
        //해당 슬롯이 보여줄 evolveId 저장
        string evolveId = config.evolveId;
        //해당 슬롯이 보여줄 value 저장
        int value = config.value;

        //evolveId로 EvolveData-정의 데이터 쪽에 조회요청필요
        //evolveDataBase 그대로 사용해서, 정의 데이터 조회
        EvolveData evolveData = EvolveDatabase.GetEvolve(evolveId);

        //해금 됐는지 여부
        bool isUnlocked = IsSlotUnlocked(slotIndex);
        //해금 가능 여부
        bool canUnlock = CanUnlockSlot(slotIndex);
        //비용부분- 임시 
        int cost = levelConfig.cost;
        //패널 갱신부분
        infoPanel.Show(level, evolveData, value, cost, isUnlocked, canUnlock);
    }

    /// <summary>
    /// 해금된 슬롯인지 판단
    /// </summary>
    /// <param name="slotindex"></param>
    /// <returns></returns>
    private bool IsSlotUnlocked(int slotindex)//**
    {
        return slotindex < playerInfoSO.evolveUnlockSlotCount;
    }
    //해금 가능한 슬롯인지 판단
    private bool CanUnlockSlot(int slotIndex)//**
    {
        //다음 슬롯인지
        bool isNextSlot = (slotIndex == playerInfoSO.evolveUnlockSlotCount);
        //상한선 계산하고
        int maxUnlockableSlots = GetMaxUnlockableSlot();
        //상한 범위 안인지 계산하고
        bool isInCap = (playerInfoSO.evolveUnlockSlotCount < maxUnlockableSlots);
        //총 슬롯 안의 범위인지 확인한 다음에?
        bool isInRange = (slotIndex >= 0 ) && (slotIndex < _totalSlotCount);
        //결과값 반환
        return isNextSlot && isInCap && isInRange;
    }
    /// <summary>
    /// 계정레벨 기준 최대 해금 가능 슬롯 수 계산
    /// -계정레벨 1당 3개
    /// -총 슬롯수를 넘지않게,
    /// </summary>
    /// <returns></returns>
    private int GetMaxUnlockableSlot()
    {
        int maxSlots = playerInfoSO.accountLevel * _slotPerLevel;
        maxSlots = Mathf.Clamp(maxSlots, 0, _totalSlotCount);
        return maxSlots;
    }

    //진화 인포패널에서 선택된 슬롯 기준 해금 버튼 호출 함수
    public void TryUnlockSelectedSlot()
    {
        TryUnlock(_selectedSlotIndex);
    }

    /// <summary>
    /// 패널에서 해금 버튼 클릭시 호출
    /// -레벨 단위 해금으로 변경되었음. ==수정할것
    /// </summary>
    /// <param name="slotIndex"></param>
    public void TryUnlock(int slotIndex)
    {
        //해금 불가면 UI 갱신만하기
        if (CanUnlockSlot(slotIndex) == false)
        {
            RefreshAll();
            return;
        }

        //해당 슬롯이 속한 레벨,인덱스 계산
        int level;
        int indexInLevel;
        ConvertSlotIndexToLevel(slotIndex, out level, out indexInLevel);

        //레벨 설정 조회부분
        EvolveLevelConfig levelConfig = EvolveLevelConfigDatabase.GetLevel(level);

        //재화 검사 부분
        int cost = levelConfig.cost;
        if (playerInfoSO.gold < cost)
        {
            //골드 부족 ui 빼먹었네..아..추가할것
            RefreshAll();
            return;
        }

        //config 추출
        EvolveConfig config = levelConfig.configs[indexInLevel];
        string evolveId = config.evolveId;
        int value = config.value;

        //정의 데이터 조회부분
        EvolveData evolveData = EvolveDatabase.GetEvolve(evolveId);

        //골드 차감
        playerInfoSO.gold = playerInfoSO.gold - cost;
        //해금 처리-해금단계 누적
        playerInfoSO.evolveUnlockSlotCount = playerInfoSO.evolveUnlockSlotCount + 1;
        //마지막 해금 단계 ID 갱신해주고
        playerInfoSO.lastEvolveId = evolveId;
        //스탯 반영
        ApplyEvolveToBaseStat(evolveData.gainStat, value);
        //저장 기점
        SaveManager.Instance.Save();
        //갱신 처리
        RefreshAll();
        //인포패널도 갱신
        OpenInfoPanel(_selectedSlotIndex);
    }

    /// <summary>
    /// 해금된 진화를 BaseStatSO에 반영하는 함수
    /// 현재 제이슨에 등록된 id
    /// Evol_Defence-방어력
    /// Evol_GetHP-고기회복량
    /// Evol_Attack -공격력
    /// Evol_Health-최대체력
    /// </summary>
    /// <param name="step"></param>
    private void ApplyEvolveToBaseStat(string gainStat, int value)
    {
        //공격력이면?
        if (gainStat == "attack")
        {
            baseStatSO.attack = baseStatSO.attack + value;
            return;
        }
        //최대체력이면?
        if (gainStat == "maxHP")
        {
            baseStatSO.maxHP = baseStatSO.maxHP + value;
            return;
        }
        //방어력이면?
        if (gainStat == "defence")
        {
            baseStatSO.defence = baseStatSO.defence + value;
            return;
        }
        //남은거 고기회복량
        baseStatSO.getHPWithMeat = baseStatSO.getHPWithMeat + value;
    }

    /// <summary>
    /// 모든 슬롯,커넥터,마커 갱신 함수 한번에 호출
    /// -해금상태에 따라 슬롯 UI 갱신
    /// -커넥터는 위아래 슬롯이 해금일때만 켜지게
    /// </summary>
    private void RefreshAll() // 이부분 분리
    {
        //슬롯부분
        RefreshSlots();
        //커넥터 부분
        RefreshConnectors();
        //마커 부분
        RefreshMarkers();
    }

    /// <summary>
    /// 슬롯 갱신
    /// </summary>
    private void RefreshSlots()
    {
        for (int slotIndex = 0; slotIndex < _totalSlotCount; slotIndex++)
        {
            //현재 슬롯 참조 가져오고
            EvolSlotButton slot = _evolSlots[slotIndex];
            //없으면 일단 다음 슬롯으로
            if (slot == null) continue;
            //현재 슬롯이 속한 레벨 저장용
            int level;
            //해당 레벨 내부에서 몇번째 슬롯인지 저장용
            int indexInLevel;
            //슬롯 인덱스를 레벨로 변환
            ConvertSlotIndexToLevel(slotIndex, out level, out indexInLevel);
            //레벨 테이블에서 현재 슬롯이 속한 레벨의 설정 가져오고
            EvolveLevelConfig levelConfig = EvolveLevelConfigDatabase.GetLevel(level);

            //해당 슬롯에 대응되는 config 가져오기
            EvolveConfig config = levelConfig.configs[indexInLevel];
            //evolveId 추출
            string evolveId = config.evolveId;

            //evolveId 정의 데이터 조회 부분
            EvolveData data = EvolveDatabase.GetEvolve(evolveId);

            //런타임 데이터 생성
            EvolveDataRuntime runtime = new EvolveDataRuntime(data);
            //해금 상태 보여줄 아이콘
            Sprite activeIcon = runtime.activeSprite;
            //미해금 상태 보여줄 아이콘
            Sprite deactiveIcon = runtime.deactiveSprite;
            //레벨단위 기반 해금되었는지 판단
            bool isUnlocked = IsSlotUnlocked(slotIndex);
            //최종 갱신
            slot.RefreshSlotUI(isUnlocked, activeIcon, deactiveIcon);
        }
    }
    /// <summary>
    /// 커넥터 갱신
    /// </summary>
    private void RefreshConnectors()
    {
        for (int i = 0; i < _connectors.Length; i++)
        {
            //현재 커넥터 참조
            GameObject connector = _connectors[i];
            //커넥터 없으면 일단 무시
            if (connector == null) continue;

            bool upperUnlocked = IsSlotUnlocked(i);
            bool lowerUnlocked = IsSlotUnlocked(i+1);

            bool on = upperUnlocked && lowerUnlocked;

            connector.SetActive(on);
        }
    }
    /// <summary>
    /// 마커 갱신
    /// 플레이어 accountlevel 기준으로,
    /// 각 레벨의 마커 활성화/비활성화 전환
    /// </summary>
    private void RefreshMarkers()
    {
        for (int level = 1; level <= _maxLevel; level++)
        {
            //배열접근, -1인덱스로 접근
            LevelMarker marker = _levelMarkers[level-1];
            //마커 없으면 일단 무시
            if(marker == null) continue;

            int required = level * _slotPerLevel;
            //현재 레벨이 해금된 상태인지 판단
            bool isActive = (playerInfoSO.accountLevel >= level);
            //마커 스프라이트 상태에따라 전환
            marker.SetActive(isActive);
        }
    }

    //마커 커넥터 코루틴 요청함수
    private void RequestRepositionCO()
    {
        if (_repositionCO != null)
        {
            StopCoroutine(_repositionCO);
            _repositionCO = null;
        }
        _repositionCO = StartCoroutine(RepositionAfterLayoutCO());
    }

    /// <summary>
    /// 레이아웃 적용 후 위지 재계산 코루틴
    /// 슬롯이랑 같은 시기에 마커랑 커넥터 생성하니까 위치를 아예 못잡음
    /// 미쳐버리겠는 상황이라 UI레이아웃이 실제 적용된 이후에
    /// 마커 커넥터 위치 갱신하기 위함.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RepositionAfterLayoutCO()
    {
        //한프레임 대기
        yield return null;
        //레이아웃 강제 확정
        Canvas.ForceUpdateCanvases();

        //한 프레임 한번 더 대기-진짜 마지막 테스트
        yield return null;
        //레이아웃 강제 확정
        Canvas.ForceUpdateCanvases();

        //마커랑 커넥터 위치 갱신
        MarkerPosSetting();

        ConnectorsPosSetting();
        
        //커넥터 sibling 설정
        SendConnetorBehind();

        _repositionCO = null;
    }

    /// <summary>
    /// +커넥터가 자꾸 슬롯 가려서 커넥터sibling 순서 정리
    /// </summary>
    private void SendConnetorBehind()
    {
        for (int i = 0; i < _connectors.Length; i++)
        {
            GameObject connector = _connectors[i];
            connector.transform.SetAsFirstSibling();
        }
    }
}
