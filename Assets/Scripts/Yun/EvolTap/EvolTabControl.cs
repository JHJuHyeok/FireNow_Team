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
/// -비용은 일단 고정
/// -비용차감은 PlayerInfoSO.gold에서,
/// -스탯적용은 BaseStatSO에 적용
/// -데이터는 ID 규칙 있을것! EvolveDatabase에서 가져오기
/// -아이콘은 SpriteAtlas 에서 가져오는데..
/// -해당부분 좀 쉽게 가려고, 아틀라스에서 스탯별 이름은 같게,
/// -미해금 아이콘은 뒤에 _locked 붙여줘야함
/// </summary>
public class EvolTabControl : MonoBehaviour
{
    [Header("스크롤뷰 컨텐트")]
    [SerializeField] private Transform content;
    [SerializeField] private ScrollRect scrollRect;

    [Header("사용 할 프리팹")]
    [SerializeField] private EvolSlotButton slotPrefab;
    [SerializeField] private GameObject connectorPrefab;

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

    //아틀라스 직접 참조
    [Header("진화 아틀라스")]
    [SerializeField] private SpriteAtlas evolveSpriteAtlas;

    //ID 규칙성 부여
    [Header("ID 규칙")]
    //고정형 접두어 규칙
    [SerializeField] private string evolveIdPrefixed = "Evol_";
    //뒤에 정수형 몇자리로 넣은건지 규칙
    [SerializeField] private int evolveIdDigits = 3;

    //++추가 레벨 마커 3레벨마다 진화 슬롯 옆에 배치위함
    [Header("레벨마커 프리팹")]
    [SerializeField] private LevelMarker markerPrefab;
    //생성된 마커 저장용 배열
    private LevelMarker[] levelMarkers;
    //몇레벨마다 배치할건지 고정수치 기준점
    private const int slotPerLevel = 3;
    //마커 x좌표는 항상 0(중앙)
    [SerializeField] private float markerX = 0.0f;

    //마커 커넥터 갱신 코루틴 중복방지
    private Coroutine repositionCO;

    //해금 상태 저장용 배열
    private bool[] unlockedState;

    //생성된 슬롯 프리팹 관리
    private EvolSlotButton[] evolSlots;
    private GameObject[] connectors;

    //마지막으로 클릭한 슬롯의 렉트 트랜스폼
    private RectTransform clickedRect;

    private void Awake()
    {
        //인포패널 레이아웃은 배치요소에서 제외
        LayoutElement layoutElement = infoPanel.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = infoPanel.gameObject.AddComponent<LayoutElement>();
        }
        layoutElement.ignoreLayout = true;

        //데이터베이스 이니셜라이즈 아직 없어서 임시로 여기서 초기화
        EvolveDatabase.Initialize();

        //해금 상태 배열 생성
        unlockedState = new bool[BasicEvolRule.totalSlot];
        //슬롯,커넥터,마커 생성
        CreateInContent();

        //패널이 해금요청 보내게 컨트롤 연결
        infoPanel.BindControl(this);
        //초기상태는 숨긴 상태로
        infoPanel.Hide();
        //UI갱신하고 시작
        RefreshAllSlots();
    }

    /// <summary>
    /// 컨텐트에 슬롯, 커넥터 프리팹 자동 생성함수
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
        evolSlots = new EvolSlotButton[BasicEvolRule.totalSlot];
        //슬롯 이어줄 커넥터 배열도 생성(생성될 슬롯 갯수-1)
        connectors = new GameObject[BasicEvolRule.totalSlot - 1];

        //슬롯 생성은 80->1 단계순으로 생성-최종적으로 1단계가 맨아래로 오게
        int total = BasicEvolRule.totalSlot;

        //++추가 레벨마커 배열 크기 계산-3슬롯마다 1개의 레벨마커
        int markerCount = total / slotPerLevel;
        levelMarkers = new LevelMarker[markerCount];

        for (int step = total; step >= 1; step--)
        {
            int slotIndex = step - 1;
            //슬롯생성
            EvolSlotButton slotInstance = Instantiate(slotPrefab, content);

            //슬롯에 인덱스/컨트롤 부여
            slotInstance.Initialize(slotIndex, this);
            //배열에 저장
            evolSlots[slotIndex] = slotInstance;

            //++추가 3슬롯 지점마다 레벨 마커 생성
            bool is3rdPoint = (step % slotPerLevel) == 0;
            if (is3rdPoint)
            {
                //마커가 표시할 레벨
                int level = step / slotPerLevel;
                //레벨은 1부터 배열시작 인덱스 생각해서 마커인덱스 저장
                int markerIndex = level - 1;

                LevelMarker markerInstance = Instantiate(markerPrefab, content);

                //배열에 저장
                levelMarkers[markerIndex] = markerInstance;
                //레이아웃에 영향받지 않게 이그노어
                LayoutElement markerLayout = markerInstance.GetComponent<LayoutElement>();
                if (markerLayout == null)
                {
                    markerLayout = markerInstance.gameObject.AddComponent<LayoutElement>();
                }
                markerLayout.ignoreLayout = true;
            }
        }
        //커넥터 부분
        for (int i = 0; i < connectors.Length; i++)
        {
            GameObject connectorInstance = Instantiate(connectorPrefab, content);

            //레이아웃 그룹이 배치요소로 취급하지 않게
            LayoutElement layoutElement = connectorInstance.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = connectorInstance.AddComponent<LayoutElement>();
            }
            layoutElement.ignoreLayout = true;

            //배열저장
            connectors[i] = connectorInstance;
        }

        //마커, 커넥터 갱신 코루틴 요청
        RequestRepositionCO();
    }

    /// <summary>
    /// 마커 위치 세팅 함수--
    /// </summary>
    private void MarkerPosSetting()
    {
        int totalSlot = evolSlots.Length;

        RectTransform contentRect = content as RectTransform;

        //뷰포트 중앙 x를 컨텐트 로컬좌표로 변환해서 가져오기
        RectTransform viewportRect = scrollRect.viewport;
        //content 로컬 좌표에서의 시각적 중앙.x
        float centerX = 0.0f;
        Vector3 viewportCenterWorld = viewportRect.TransformPoint(viewportRect.rect.center);
        Vector3 viewportCenterLocal = contentRect.InverseTransformPoint(viewportCenterWorld);
        centerX = viewportCenterLocal.x;

        for (int i = 0; i < levelMarkers.Length; i++)
        {
            LevelMarker marker = levelMarkers[i];

            int Slot3rdIndex = totalSlot - ((i + 1) * slotPerLevel);

            //음수로 내려가면 방어
            if (Slot3rdIndex <0 || Slot3rdIndex >= totalSlot) continue;
            //3번째 기준 슬롯 저장
            EvolSlotButton slot = evolSlots[Slot3rdIndex];
            if (slot == null) continue;
            //슬롯 마커 렉트 트랜스폼
            RectTransform slotRect = slot.transform as RectTransform;
            RectTransform markerRect = marker.transform as RectTransform;

            //슬롯 인덱스로 계산, 아래부터 1
            int displayLevel = (Slot3rdIndex + 1) / slotPerLevel;
            marker.SetLevel(displayLevel);

            //활성기준 = 현재 계정레벨이하만 활성화로 표시 -테스트플레이어 레벨 쓰는중(임시)
            bool isActive = displayLevel <= playerInfoSO.accountLevel;
            marker.SetActive(isActive);

            //혹시 앵커,피벗이 지멋대로 바뀌나 싶어서 강제
            markerRect.anchorMin = new Vector2 (0.5f, 1.0f);
            markerRect.anchorMax = new Vector2 (0.5f, 1.0f);
            markerRect.pivot = new Vector2 (0.5f, 0.5f);

            //슬롯의 실제 표시 위치를 content 로컬좌표로 획득
            Vector2 slotLocal = GetContentLocalPoint(slotRect);
            //앵커포지션 대신 로컬포지션으로 y맞추기
            Vector3 markerLocal = markerRect.localPosition;
            markerLocal.x = centerX;
            markerLocal.y = slotLocal.y;
            markerLocal.z = 0.0f;
            markerRect.localPosition = markerLocal;
        }
    }

    /// <summary>
    /// 커넥터 위치세팅 함수--
    /// </summary>
    private void ConnectorsPosSetting()
    {
        //커넥터 전체를 순회하면서 위치/길이를 갱신
        for (int i = 0; i < connectors.Length; i++)
        {
            //i번째 커넥터 지정
            GameObject connector = connectors[i];

            //최소방어
            if (connector == null) continue;

            //해당 커넥터가 연결해야 할 슬롯들의 RectTransform계산
            //슬롯A(이전)
            RectTransform slotA = evolSlots[i].transform as RectTransform;
            //슬롯B(다음)
            RectTransform slotB = evolSlots[i + 1].transform as RectTransform;
            //커넥터의 RectTransform계산
            RectTransform connectorRect = connector.transform as RectTransform;

            //커넥터가 있어야할 위치 계산
            Vector2 a = GetContentLocalPoint(slotA);
            Vector2 b = GetContentLocalPoint(slotB);
            Vector2 middle = (a + b) * 0.5f;
            //커넥터를 해당 위치로 이동
            connectorRect.anchoredPosition = middle;
        }
    }

    /// <summary>
    /// 슬롯 클릭시 슬롯 버튼쪽에서 이벤트 호출
    /// -클릭 슬롯 위치 저장, 패널위치 슬롯위로 이동, 패널내용 갱신후 표시
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="slotRect"></param>
    public void OnClickSlot(int slotIndex, RectTransform slotRect)
    {
        //마지막 슬롯 위치 저장
        clickedRect = slotRect;
        //패널을 할상 가장 위로 보이도록 마지막 자식으로 보내버리기
        infoPanel.transform.SetAsLastSibling();
        //패널을 슬롯위로
        PanelOnSlot(slotRect);
        //패널 오픈
        OpenInfoPanel(slotIndex);
    }

    /// <summary>
    /// 패널 위치를 슬롯위로 위치하게 하는 함수
    /// </summary>
    /// <param name="slotRect"></param>
    private void PanelOnSlot(RectTransform slotRect)
    {
        Vector2 pos = slotRect.anchoredPosition;
        //지정값만큼 위로 올려주고
        pos.y = pos.y + infoPanelY;
        //패널위치 적용
        infoPanelRect.anchoredPosition = pos;
    }

    /// <summary>
    /// 슬롯 인덱스 기반으로 패널갱신
    /// -EvolveDatabase에서 (규칙된 ID)로 EvolveData 찾기(진짜중요)
    /// -단계 기반 스탯 타입과 수치 계산
    /// -해금 여부,해금가능여부에 따라 버튼 표시 제어
    /// </summary>
    /// <param name="slotIndex"></param>
    private void OpenInfoPanel(int slotIndex)
    {
        //step 쪽이 
        int step = slotIndex + 1;
        //해당단계 evloveId 는 규칙으로 생성
        string evolveId = StepToEvolveId(step);
        //DB에서 ID로 데이터 조회
        EvolveData data = EvolveDatabase.GetEvolve(evolveId);
        //스탯타입은 단계로 계산
        BasicEvolveStatType statType = BasicEvolRule.GetStatTypeByStep(step);
        //수치증가량은 고정(임시)
        int amount = BasicEvolRule.GetIncreaseAmountByStep();
        //해금여부는 배열 참조
        bool isUnlocked = unlockedState[slotIndex];
        //해금 가능여부 조건 검사
        bool canUnlock = CheckCanUnlock(slotIndex, playerInfoSO.accountLevel);
        //패널 갱신부분
        infoPanel.Show(slotIndex, data, statType, amount, isUnlocked, canUnlock);
    }

    /// <summary>
    /// 패널에서 해금 버튼 클릭시 호출
    /// 
    /// </summary>
    /// <param name="slotIndex"></param>
    public void TryUnlock(int slotIndex)
    {
        //해금 상태면 패널,슬롯 갱신만하기
        if (unlockedState[slotIndex] == true)
        {
            RefreshAllSlots();
            OpenInfoPanel(slotIndex);
            return;
        }

        //조건 검사 부분
        bool canUnlock = CheckCanUnlock(slotIndex, playerInfoSO.accountLevel);
        if (canUnlock == false)
        {
            RefreshAllSlots();
            OpenInfoPanel(slotIndex);
            return;
        }

        //재화 검사 부분
        int cost = BasicEvolRule.unlockEvolveCost;
        if (playerInfoSO.gold < cost)
        {
            //골드 부족 ui 빼먹었네..아..추가할것
            RefreshAllSlots();
            OpenInfoPanel(slotIndex);
            return;
        }

        //골드 차감
        playerInfoSO.gold = playerInfoSO.gold - cost;
        //해금 처리
        unlockedState[slotIndex] = true;
        //스탯 반영
        int step = slotIndex + 1;
        ApplyEvolveToBaseStat(step);
        //갱신 처리
        RefreshAllSlots();
        OpenInfoPanel(slotIndex);
    }

    /// <summary>
    /// 해금 가능 조건 검사
    /// -레벨*3관련
    /// -이전 슬롯 해금여부
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="playerLevel"></param>
    /// <returns></returns>
    private bool CheckCanUnlock(int slotIndex, int playerLevel)
    {
        int step = slotIndex + 1;

        //레벨로 열린 최대 단계 수
        int openStepCount = BasicEvolRule.GetStepCountByLevel(playerLevel);
        //레벨부족이면 false
        if (openStepCount < step) return false;
        //1단계는 이전 슬롯 조건없고
        if (slotIndex == 0) return true;
        //이전슬롯 미해금이면 false
        if (unlockedState[slotIndex - 1] == false) return false;

        return true;
    }

    /// <summary>
    /// 해금된 진화를 BaseStatSO에 반영하는 함수
    /// </summary>
    /// <param name="step"></param>
    private void ApplyEvolveToBaseStat(int step)
    {
        //스탯타입 계산후 저장
        BasicEvolveStatType statType = BasicEvolRule.GetStatTypeByStep(step);
        //증가수치 저장
        int amount = BasicEvolRule.GetIncreaseAmountByStep();
        //공격력이면?
        if (statType == BasicEvolveStatType.attack)
        {
            baseStatSO.attack = baseStatSO.attack + amount;
            return;
        }
        //최대체력이면?
        if (statType == BasicEvolveStatType.maxHP)
        {
            baseStatSO.maxHP = baseStatSO.maxHP + amount;
            return;
        }
        //방어력이면?
        if (statType == BasicEvolveStatType.defence)
        {
            baseStatSO.defence = baseStatSO.defence + amount;
            return;
        }
        //남은거 고기회복량
        baseStatSO.getHPWithMeat = baseStatSO.getHPWithMeat + amount;
    }

    /// <summary>
    /// 모든 슬롯,커넥터 갱신 함수
    /// -각 단계의 ID는 규칙으로 생성, EvolveDataBase에서 data 조회
    /// -data.spriteName 기반으로 아틀라스에서 spriteName/spriteName_locked 로드
    /// -해금상태에 따라 슬롯 UI 갱신
    /// -커넥터는 위아래 슬롯이 해금일때만 켜지게
    /// </summary>
    private void RefreshAllSlots()
    {
        //슬롯 부분
        for (int i = 0; i < evolSlots.Length; i++)
        {
            EvolSlotButton slot = evolSlots[i];

            if (slot == null) continue;

            int step = i + 1;

            string evolveId = StepToEvolveId(step);

            EvolveData data = EvolveDatabase.GetEvolve(evolveId);

            string spriteName =(data!=null) ? data.spriteName : null;

            Sprite unlockedIcon = evolveSpriteAtlas.GetSprite(spriteName);

            Sprite lockedIcon = evolveSpriteAtlas.GetSprite(spriteName + "_locked");

            bool isUnlocked = unlockedState[i];

            slot.RefreshSlotUI(isUnlocked, unlockedIcon, lockedIcon);
        }
        //커넥터 부분
        for (int i = 0; i < connectors.Length; i++)
        {
            GameObject connector = connectors[i];

            if (connector == null) continue;

            bool on = (unlockedState[i] == true) && (unlockedState[i + 1] == true);

            connector.SetActive(on);
        }
    }

    /// <summary>
    /// (1~80)단계를 ID 규칙 문자열로 변환
    /// 1단계 진화 => ID : Evol_001
    /// 9단계 진환 => ID : Evol_009
    /// 무조건 진짜 통일성있게, 앞에는 고정형,
    /// 뒤에는 자릿수 동일하게하며, 숫자 순서대로.
    /// 여기가 돌아가려면 Json의 Id도 반드시,
    /// 여기의 Id결과물과 같도록 만들어줘야함!
    /// </summary>
    /// <param name="step"></param>
    /// <returns></returns>
    private string StepToEvolveId(int step)
    {
        //자릿수를 문자열로 변환
        string digitString = evolveIdDigits.ToString();
        //C# 숫자포맷 D 사용(decimal(10진수))
        //"D3" = 왼쪽3자리는 0으로 채우기
        // 1.Tostring("D3") =>"001" / 9.Tostring("D3") =>"009"
        string format = "D" + digitString;
        //step을 포맷대로 문자열로 변환
        //항상 3자리로 맞추는게 포인트!
        string numberPart = step.ToString(format);
        //접두어, 숫자 문자열 붙여서 최종 ID 만들기
        string id = evolveIdPrefixed + numberPart;
        return id;
    }

    //마커 커넥터 코루틴 요청함수
    private void RequestRepositionCO()
    {
        if (repositionCO != null)
        {
            StopCoroutine(repositionCO);
            repositionCO = null;
        }
        repositionCO = StartCoroutine(RepositionAfterLayoutCO());
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

        RectTransform contentRect = content as RectTransform;

        //한 프레임 한번 더 대기-진짜 마지막 테스트
        yield return null;
        //레이아웃 강제 확정
        Canvas.ForceUpdateCanvases();

        //마커랑 커넥터 위치 갱신
        MarkerPosSetting();
        ConnectorsPosSetting();
        //커넥터 sibling 설정
        SendConnetorBehind();

        repositionCO = null;
    }

    /// <summary>
    /// rectTransform의 실제 화면 중심점을
    /// content 로컬좌표로 변환해서 반환하는 헬퍼 함수
    /// </summary>
    /// <returns></returns>
    private Vector2 GetContentLocalPoint(RectTransform targetRect)
    {
        RectTransform contentRect = content as RectTransform;
        if (contentRect == null || targetRect == null)
        {
            return Vector2.zero;
        }
        //targetRect 기준 월드 좌표
        Vector3 worldCenter = targetRect.TransformPoint(targetRect.rect.center);
        //content 기준 로컬좌표로 변환
        Vector3 local = contentRect.InverseTransformPoint(worldCenter);

        return new Vector2(local.x, local.y);
    }

    /// <summary>
    /// +커넥터가 자꾸 슬롯 가려서 커넥터sibling 순서 정리
    /// </summary>
    private void SendConnetorBehind()
    {
        for (int i = 0; i < connectors.Length; i++)
        {
            GameObject connector = connectors[i];
            connector.transform.SetAsFirstSibling();
        }
    }
}
