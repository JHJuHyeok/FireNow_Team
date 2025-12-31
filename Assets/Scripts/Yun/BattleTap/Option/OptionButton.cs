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

    private void Awake()
    {
        optionButton.onClick.AddListener(OpenOptionPanel);
        exitButton.onClick.AddListener(CloseOptionPanel);
    }
    /// <summary>
    /// 옵션 버튼 클릭시 옵션패널 활성화
    /// </summary>
    private void OpenOptionPanel()
    {
        SoundManager.Instance.PlaySound("ButtonClick");
        optionCanvas.SetActive(true);
    }
    /// <summary>
    /// 옵션 패널내 닫기 버튼 누를시 패널 비활성화
    /// </summary>
    private void CloseOptionPanel()
    {
        SoundManager.Instance.PlaySound("ClosePopup");
        optionCanvas.SetActive(false);
    }
}
