using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//옵션 패널 활성화 기능
public class OptionButton : MonoBehaviour
{
    [Header("온오프 될 옵션패널")]
    [SerializeField] private GameObject optionCanvas;

    [Header("설정열기 버튼")]
    [SerializeField] private Button optionButton;

    [Header("설정패널닫기 버튼")]
    [SerializeField] private Button exitButton;

    //어웨이크에서 버튼 이벤트 등록
    private void Awake()
    {
        optionButton.onClick.AddListener(OpenOptionPanel);
        exitButton.onClick.AddListener(CloseOptionPanel);
    }
    //패널 열기 함수
    private void OpenOptionPanel()
    {
        SoundManager.Instance.PlaySound("ButtonClick");
        optionCanvas.SetActive(true);
    }
    //패널 닫기 함수
    private void CloseOptionPanel()
    {
        SoundManager.Instance.PlaySound("ClosePopup");
        optionCanvas.SetActive(false);
    }
}
