using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//개신기한거 보고옴
//메세지 출력 필요할때 토스트기 처럼 뿅하고 튀어나오게 하는
//어디서든 텍스트 불러오는 메시지 표시기
//공용으로 여러곳에서 사용가능 -준희님께 여쭤보고 필요하시면 쓰시라고 하자
public class UIToast : MonoBehaviour
{
    private static UIToast instance;

    [Header("표시할 텍스트")]
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("자동 숨김 시간")]
    [SerializeField] private float hideDuration = 1.0f;

    //현재 실행중인 텍스트 자동 숨김 코루틴 저장용
    private Coroutine hideCo;

    private void Awake()
    {
        //중복방지
        if (instance != null && instance != this)
        {
            Destroy(instance);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //보여주기 기본형태
    public static void ShowText(string message)
    {
        if (instance == null) return;
        instance.ShowTextRogic(message, instance.hideDuration);
    }

    //보여주기 특정 시간지정형태
    public static void ShowText(string message, float duration)
    {
        if (instance == null) return;
        instance.ShowTextRogic(message, duration);
    }

    //실제 텍스트 표시 로직
    private void ShowTextRogic(string message, float duration)
    {
        //텍스트 표시공간 연결안되있으면 중지
        if (messageText == null) return;

        //텍스트 표시공간 활성화
        messageText.gameObject.SetActive(true);
        //텍스트 표시공간에 메세지 적용
        messageText.text = message;

        //기존 코루틴 돌고 있으면 중단
        if (hideCo != null)
        {
            StopCoroutine(hideCo);
            hideCo = null;
        }
        //텍스트 자동 숨김 코루틴 시작
        hideCo = StartCoroutine(HideTextCO(duration));
    }
    
    //몇초뒤에 텍스트 숨길지 코루틴
    private IEnumerator HideTextCO(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        //시간 지나면 숨김처리
        HideRightNow();
    }

    //즉시 텍스트 숨기기
    private void HideRightNow()
    {
        //텍스트 표시공간 연결 안되어 있으면 중지
        if (messageText == null) return;

        messageText.gameObject.SetActive(false);
        //텍스트 표시공간도 비워주고
        messageText.text = string.Empty;
        //hideCo 남아있으면 정리
        hideCo = null;
    }
}
