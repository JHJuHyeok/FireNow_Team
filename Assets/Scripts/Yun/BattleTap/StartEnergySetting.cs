using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 배틀탭=> 게임시작버튼 위 에너지 배수 설정 클래스
/// </summary>
public class StartEnergySetting : MonoBehaviour
{
    [Header("배수 설정 버튼")]
    [SerializeField] private Button staminaButton;

    [Header("배수 설정 버튼 텍스트")]
    [SerializeField] private TextMeshProUGUI staminaText;

    [Header("토글이 될 루트 오브젝트")]
    [SerializeField] GameObject staminaPanel;


    [Header("배수 버튼")]
    [SerializeField] private Button x1Button;
    [SerializeField] private Button x2Button;

    [Header("배수 버튼 이미지")]
    [SerializeField] private Image x1Image;
    [SerializeField] private Image x2Image;

    [Header("선택된 버튼에 부여할 이미지")]
    [SerializeField] private Sprite yellowSprite;
    [SerializeField] private Sprite graySprite;

    [Header("게임 시작 버튼 코스트 텍스트")]
    [SerializeField] private TextMeshProUGUI startCostText;

    [Header("배수별 실제 적용 코스트 값")]
    [SerializeField] private int x1Cost = 5;
    [SerializeField] private int x2Cost = 10;

    //=====선택 상태 저장 변수=====

    //현재 선택된 배수 저장용(초기 1상태)
    public int focusedMutiplier { get; private set; } = 1;
    public int selectedCost => (focusedMutiplier == 1)? x1Cost : x2Cost;

    private void Awake()
    {
        staminaButton.onClick.AddListener(TogglePanel);
        x1Button.onClick.AddListener(()=> SelectMultiplierButton(1));
        x2Button.onClick.AddListener(()=> SelectMultiplierButton(2));
    }

    private void Start()
    {
        ClosePanel();
        RefreshUI();
    }

    /// <summary>
    /// (패널이 열린상태에서) 배수설정 버튼 클릭시 패널활성화
    /// </summary>
    private void OpenPanel()
    {
        SoundManager.Instance.PlaySound("OpenPopup");
        staminaPanel.SetActive(true);
    }
    /// <summary>
    /// (패널이 닫힌상태에서) 배수설정 버튼 클릭시, 패널 비활성화
    /// </summary>
    private void ClosePanel()
    {
        SoundManager.Instance.PlaySound("ClosePopup");
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
        if (IsPanelOpen()) ClosePanel();
        else OpenPanel();
    }
    /// <summary>
    /// 각 배수 버튼에 맞는 값으로 필요비용UI변경
    /// </summary>
    /// <param name="multiplier"></param>
    private void SelectMultiplierButton(int multiplier)
    {
        SoundManager.Instance.PlaySound("ButtonClick");
        //multiplier 가 2면 focusedMutiplier를 2로
        focusedMutiplier = (multiplier == 2) ? 2 : 1;
        //선택된 상태에 따라 UI갱신
        RefreshUI();
        //버튼 눌렀을때도 패널이 닫혀야됨
        ClosePanel();
    }

    /// <summary>
    /// 선택 상태 UI 갱신 종합
    /// </summary>
    private void RefreshUI()
    {
        ChangeButtonImage();
        ChangeButtonText();
        ChangeStartCostText();
    }

    /// <summary>
    /// 선택된 상태에 따라 배수버튼 스프라이트 변경
    /// </summary>
    private void ChangeButtonImage()
    {
        x1Image.sprite = (focusedMutiplier == 1) ? yellowSprite : graySprite;
        x2Image.sprite = (focusedMutiplier == 2) ? yellowSprite : graySprite;
    }

    /// <summary>
    /// 선택된 상태에 따라 배수 텍스트 변경
    /// </summary>
    private void ChangeButtonText()
    {
        staminaText.text = (focusedMutiplier == 2) ? "에너지 2배" : "에너지 1배";
    }
    /// <summary>
    /// 선택된 상태에 따라 시작버튼의 비용 텍스트 변경
    /// </summary>
    private void ChangeStartCostText()
    {
        startCostText.text = $"{selectedCost}";
    }
}
