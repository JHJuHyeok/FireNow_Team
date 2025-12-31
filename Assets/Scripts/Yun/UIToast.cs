using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 화면 중앙에 텍스트 표시, 자동 비활성화 UI토스트
/// 어느씬에서도 공용으로 사용가능
/// </summary>
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

    /// <summary>
    /// 디폴트 시간 텍스트 표시
    /// </summary>
    /// <param name="message"></param>
    public static void ShowText(string message)
    {
        if (instance == null) return;
        instance.ShowTextRogic(message, instance.hideDuration);
    }

    /// <summary>
    /// 사용자 지정시간 텍스트 표시
    /// </summary>
    /// <param name="message"></param>
    /// <param name="duration"></param>
    public static void ShowText(string message, float duration)
    {
        if (instance == null) return;
        instance.ShowTextRogic(message, duration);
    }

    /// <summary>
    /// 텍스트 표시 로직
    /// 텍스트 활성화 이후 지정 시간이후 자동 비활성화
    /// </summary>
    /// <param name="message"></param>
    /// <param name="duration"></param>
    private void ShowTextRogic(string message, float duration)
    {
        if (messageText == null) return;

        messageText.gameObject.SetActive(true);
        messageText.text = message;

        //기존 코루틴 중복방지
        if (hideCo != null)
        {
            StopCoroutine(hideCo);
            hideCo = null;
        }
        hideCo = StartCoroutine(HideTextCO(duration));
    }

    /// <summary>
    /// 시간포함 텍스트 비활성화
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    private IEnumerator HideTextCO(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        //시간 지나면 숨김처리
        HideRightNow();
    }

    /// <summary>
    /// 시간 미포함 텍스트 비활성화
    /// </summary>
    private void HideRightNow()
    {
        if (messageText == null) return;

        messageText.gameObject.SetActive(false);
        messageText.text = string.Empty;
        hideCo = null;
    }
}
