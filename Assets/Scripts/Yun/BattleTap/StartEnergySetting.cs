using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


//게임시작 버튼과 상호작용
public class StartEnergySetting : MonoBehaviour
{
    //메인메뉴에서 '배수 설정 버튼'
    [Header("배수 설정 버튼")]
    [SerializeField] private Button staminaButton;
    //배수 설정 버튼안의 텍스트 변경
    [Header("배수 설정 버튼 텍스트")]
    [SerializeField] private TextMeshProUGUI staminaText;
    //에너지 설정 패널을 포함한 루트 오브젝트
    [Header("토글이 될 루트 오브젝트")]
    [SerializeField] GameObject staminaPanel;

    //1배 2배 버튼
    [Header("배수 버튼")]
    [SerializeField] private Button x1Button;
    [SerializeField] private Button x2Button;

    //1배 2배 버튼 이미지 
    [Header("배수 버튼 이미지")]
    [SerializeField] private Image x1Image;
    [SerializeField] private Image x2Image;

    //선택/미선택 된 버튼에 부여할 이미지
    [Header("선택된 버튼에 부여할 이미지")]
    [SerializeField] private Sprite yellowSprite;
    [SerializeField] private Sprite graySprite;

    //게임 시작 버튼에 표시되는 에너지 코스트 텍스트
    [Header("게임 시작 버튼 코스트 텍스트")]
    [SerializeField] private TextMeshProUGUI startCostText;

    //배수 선택시 실제로 적용될 코스트 값
    [Header("배수별 실제 적용 코스트 값")]
    [SerializeField] private int x1Cost = 5;
    [SerializeField] private int x2Cost = 10;

    //=====선택 상태 저장 변수=====

    //현재 선택된 배수 저장용(초기 1상태)
    public int focusedMutiplier { get; private set; } = 1;
    //현재 선택된 배수가 1? 트루면 코스트1
    public int selectedCost => (focusedMutiplier == 1)? x1Cost : x2Cost;

    private void Awake()
    {
        //배수 설정 버튼 이벤트
        staminaButton.onClick.AddListener(TogglePanel);
        //1배 버튼 이벤트
        x1Button.onClick.AddListener(()=> SelectMultiplierButton(1));
        //2배 버튼 이벤트
        x2Button.onClick.AddListener(()=> SelectMultiplierButton(2));
    }

    private void Start()
    {
        //패널은 닫은상태로
        ClosePanel();
        //UI초기값으로 갱신
        RefreshUI();
    }

    /// <summary>
    /// 패널 열기
    /// </summary>
    private void OpenPanel()
    {
        staminaPanel.SetActive(true);
    }
    /// <summary>
    /// 패널 닫기
    /// </summary>
    private void ClosePanel()
    {
        staminaPanel.SetActive(false);
    }

    /// <summary>
    /// 패널 열린상태 기준점
    /// </summary>
    /// <returns></returns>
    private bool IsPanelOpen()
    {
        //루트오브젝트의 활성화 상태를 반환
        return staminaPanel.activeSelf;
    }

    /// <summary>
    /// 패널 토글기능
    /// </summary>
    private void TogglePanel()
    {
        //패널이 열려있으면 닫고, 닫혀있으면 여는 토글
        if (IsPanelOpen()) ClosePanel();
        else OpenPanel();
    }
    /// <summary>
    /// 1배 2배 버튼 눌렀을때 기능
    /// </summary>
    /// <param name="multiplier"></param>
    private void SelectMultiplierButton(int multiplier)
    {
        //multiplier 가 2면 focusedMutiplier를 2로
        focusedMutiplier = (multiplier == 2) ? 2 : 1;
        //선택된 상태에 따라 UI갱신
        RefreshUI();
        //버튼 눌렀을때도 패널이 닫혀야됨
        ClosePanel();
    }
    //선택 상태 UI 갱신 종합
    private void RefreshUI()
    {
        ChangeButtonImage();
        ChangeButtonText();
        ChangeStartCostText();
    }
    /// <summary>
    /// 배수 버튼의 이미지 바꿔줄 함수
    /// </summary>
    private void ChangeButtonImage()
    {
        x1Image.sprite = (focusedMutiplier == 1) ? yellowSprite : graySprite;
        x2Image.sprite = (focusedMutiplier == 2) ? yellowSprite : graySprite;
    }

    /// <summary>
    /// 배수 설정 버튼의 텍스트 바꿔줄 함수
    /// </summary>
    private void ChangeButtonText()
    {
        staminaText.text = (focusedMutiplier == 2) ? "에너지 2배" : "에너지 1배";
    }
    /// <summary>
    /// 게임시작 버튼의 코스트 텍스트 바꿔줄 함수
    /// </summary>
    private void ChangeStartCostText()
    {
        startCostText.text = $"{selectedCost}";
    }
}
