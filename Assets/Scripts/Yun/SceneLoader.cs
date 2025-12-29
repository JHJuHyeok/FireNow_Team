using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//연출 순서는 FadeOut -> 씬전환 ->FadeIn 
//싱글톤 필수-(연출 캔버스 포함)
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
        //FadeOut 먼저
        fadeInOut.FadeOut();
        //연출 시간만큼 기다리게 하고 싶은데,
        yield return new WaitForSeconds(fadeInOut.FadeOutDuration);
        //씬전환 때리고,
        SceneManager.LoadScene(sceneName);
        //FadeIn 추후
        fadeInOut.FadeIn();
        yield return new WaitForSeconds(fadeInOut.FadeInDuration);
    }
}
