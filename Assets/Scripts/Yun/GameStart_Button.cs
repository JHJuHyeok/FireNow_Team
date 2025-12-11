using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//게임시작 버튼 누르면 동작할 기능
//1.보유 스태미나 감소 시켜줘야되고,->아직 데이터 API 정보 모름
//2.씬로더 온클릭에 지정해주면 되고,
public class GameStart_Button : MonoBehaviour
{
    [Header("전환할 씬 이름")]
    [SerializeField] private string sceneName;

    public void OnClickStart()
    {
        //데이터에서 보유 스태미나 감소 뒤,

        //효과 포함한 씬로더 호출
        SceneLoader.Instance.LoadSceneWithFx(sceneName);
    }
}
