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
/// 
/// 커넥터 부분 해결안됨.. 이어서 할것. 다른방법을 찾던가 해야돼**
/// </summary>
public class EvolTabControl : MonoBehaviour
{
    [Header("스크롤뷰 컨텐트")]
    [SerializeField] private Transform content;

    [Header("사용 할 프리팹")]
    [SerializeField] private EvolSlotButton slotPrefab;
    [SerializeField] private GameObject connectorPrefab; //이거 준희님 만들어두신거 쓸지 고민좀

    //위치 가져와야돼 버튼 위에 똑 하고 생길거니까(고정형 아님)
    [Header("진화 인포 패널 관련")]
    [SerializeField] private EvolInfoPanel infoPanel;
    [SerializeField] private RectTransform infoPanelRect;
    //얼마나 높이띄울지는 인스펙터에 조절 가능하게
    [SerializeField] private float infoPanelY = 10.0f;

    [Header("플레이어 데이터-재화부분")]
    [SerializeField] private PlayerInfoSO playerInfoSO;

    [Header("플레이어 데이터-스탯부분")]
    [SerializeField] private BaseStatSO baseStatSO;

    //이 부분 주혁님이 info쪽에 필드 추가하신다고 함 일단 테스트용으로 사용하고 지워버려
    [Header("플레이어 레벨 지정-테스트용")]
    [SerializeField] private int testPlayerLevel = 1;

    //아틀라스 직접 참조
    [Header("진화 아틀라스")]
    [SerializeField] private SpriteAtlas evolveSpriteAtlas;

    //ID 규칙성 부여
    [Header("ID 규칙")]
    //고정형 접두어 규칙
    [SerializeField] private string evolveIdPrefixed = "Evol_";
    //뒤에 정수형 몇자리로 넣은건지 규칙
    [SerializeField] private int evolveIdDigits = 3;

    //해금 상태 저장용 배열
    private bool[] unlockedState;

    //생성된 슬롯 프리팹 관리
    private EvolSlotButton[] evolSlots;
    private GameObject[] connectors;

    //마지막으로 클릭한 슬롯의 렉트 트랜스폼
    private RectTransform clickedRect;

    private void Awake()
    {
        //인포패널 레이아웃 배치요소에서 제외
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
        //슬롯,커넥터 생성
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
            if (child.gameObject == infoPanel.gameObject)
            {
                continue;
            }
            
            Destroy(child.gameObject);
        }

        //슬롯 배열 생성
        evolSlots = new EvolSlotButton[BasicEvolRule.totalSlot];
        //슬롯 이어줄 커넥터 배열도 생성(생성될 슬롯 갯수-1)
        connectors = new GameObject[BasicEvolRule.totalSlot - 1];

        //슬롯 생성은 80->1 단계순으로 생성-최종적으로 1단계가 맨아래로 오게
        int total = BasicEvolRule.totalSlot;

        for (int step = total; step >= 1; step--)
        {
            int slotIndex = step - 1;
            //슬롯생성
            EvolSlotButton slotInstance = Instantiate(slotPrefab, content);

            //슬롯에 인덱스/컨트롤 부여
            slotInstance.Initialize(slotIndex, this);
            //배열에 저장
            evolSlots[slotIndex] = slotInstance;
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

        //컨텐트 사이즈 피터 계산 강제
        Canvas.ForceUpdateCanvases();
        //커넥터 위치 세팅
        ConnectorsPosSetting();
    }

    //커넥터 위치세팅 함수(컨텐트안의 배치요소로 취급x 특정위치에 머물기 위함)
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
            RectTransform slotA;
            //슬롯B(다음)
            RectTransform slotB;
            slotA = evolSlots[i].transform as RectTransform;
            slotB = evolSlots[i + 1].transform as RectTransform;

            //커넥터의 RectTransform계산
            RectTransform connectorRect = connector.transform as RectTransform;

            //커넥터가 있어야할 위치 계산
            Vector2 middle;
            middle = (slotA.anchoredPosition + slotB.anchoredPosition) * 0.5f;

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
        bool canUnlock = CheckCanUnlock(slotIndex, testPlayerLevel);
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
        bool canUnlock = CheckCanUnlock(slotIndex, testPlayerLevel);
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

            string spriteName =(data!=null) ? data.activeSpriteName : null;

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
}
