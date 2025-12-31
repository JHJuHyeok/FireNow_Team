using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//연출 순서는 FadeOut -> 씬전환 ->FadeIn 
//싱글톤 필수-(연출 캔버스 포함)

/// <summary>
/// 아이리스 아웃 연출을 포함한 씬로더
/// 메인메뉴->전투씬
/// 전투씬->메인메뉴 공용으로 사용가능
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [Header("연출 맡을 캔버스 오브젝트")]
    [SerializeField] private FadeINOut fadeInOut;

    //연출 중복방지 플래그
    private bool _isLoading;

    private void Awake()
    {
        //중복방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        //연출 담당 오브젝트도 유지되게
        if (fadeInOut != null)
        {
            DontDestroyOnLoad(fadeInOut.gameObject);
        }
    }

    //씬 전환(트윈을 포함한)
    public void LoadSceneWithFx(string sceneName)
    {
        //연출 중이면 중지
        if (_isLoading) return;

        StartCoroutine(LoadSceneCo(sceneName));
    }

    private IEnumerator LoadSceneCo(string sceneName)
    {
        _isLoading = true;
        
        //연출 시작 부분
        fadeInOut.FadeOut();
        yield return new WaitForSeconds(fadeInOut.FadeOutDuration);
        
        SceneManager.LoadScene(sceneName);
        
        //연출 끝 부분
        fadeInOut.FadeIn();
        yield return new WaitForSeconds(fadeInOut.FadeInDuration);
    }
}
