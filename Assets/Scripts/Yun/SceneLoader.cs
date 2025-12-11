using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//두 트윈-줌인 줌아웃 효과 들어오면서 동시에 씬전환
//줌인-씬 전환-줌아웃 되야되니까, 씬 전환 시기에, 씬로더가 파괴되면 안되는 구조-싱글톤
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //씬 전환(트윈을 포함한)
    public void LoadSceneWithFx(string sceneName)
    {
        StartCoroutine(LoadSceneCo(sceneName));
    }

    private IEnumerator LoadSceneCo(string sceneName)
    {
        //마스킹 이미지 줌인 효과->외부호출(예정)

        //씬전환 때리고,
        SceneManager.LoadScene(sceneName);
        yield return null;

        //마스킹 이미지 줌아웃 효과->외부호출(예정)
    }
}
