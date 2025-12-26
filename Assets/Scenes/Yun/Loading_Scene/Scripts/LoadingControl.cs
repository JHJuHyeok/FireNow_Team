using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingControl : MonoBehaviour
{
    [Header("연결할 로딩바 UI")]
    [SerializeField] private Slider loadingBar;

    private void Start()
    {
        //비동기로 씬을 불러오는 함수 실행
        StartCoroutine(ILoadSceneAsync());
    }

    //비동기 씬 전환 코루틴
    IEnumerator ILoadSceneAsync()
    {
        //진행 사항을 AsyncOperation으로 반환 씬 로드 방식은 비동기(Async)
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("MainMenu_Scene");

        //로드 완료시 전환 플래그
        asyncOperation.allowSceneActivation = false;

        //로딩이 끝날때까지 반복
        while (!asyncOperation.isDone)
        {
            //로딩바 움직이는거 자체는 90퍼까지만,
            float loadingProgress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            //로딩바 값을 현재 진행율 맞게 갱신
            loadingBar.value = loadingProgress;

            //1에 도달하면 씬전환 true
            if (loadingProgress >= 1.0f)
            {
                asyncOperation.allowSceneActivation = true;
            }

            //한 프레임 대기 후 반복
            yield return null;
        }
    }
}
