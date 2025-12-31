using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//접근불가 팝업창 활성화 기능-어느 버튼에서도 온클릭으로 호출 가능
public class UnavailablePopup : MonoBehaviour
{
    [Header("온오프 될 팝업창")]
    [SerializeField] private GameObject unavailablePopup;

    [Header("확인버튼")]
    [SerializeField] private Button checkButton;

    //어웨이크에서 버튼 이벤트 등록
    private void Awake()
    {
        checkButton.onClick.AddListener(CloseUnavailablePopup);
    }
    //팝업 열기 함수
    public void OpenUnavailablePopup()
    {
        SoundManager.Instance.PlaySound("Alert");
        unavailablePopup.SetActive(true);
    }
    //팝업 닫기 함수
    private void CloseUnavailablePopup()
    {
        SoundManager.Instance.PlaySound("ClosePopup");
        unavailablePopup.SetActive(false);
    }
}
