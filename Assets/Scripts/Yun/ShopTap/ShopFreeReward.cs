using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 관련 애니메이션 연출과 독립적으로 작동할 기능 클래스
/// 상점탭내 골드,젬,재료 무료획득 버튼의 기능 담당
/// 애니메이션 연출이 끝나가는 시점은 직접 딜레이로 잡아서 ui갱신
/// </summary>
public class ShopFreeReward : MonoBehaviour
{
    [Header("플레이어 데이터")]
    [SerializeField] private PlayerInfoSO PlayerInfo;

    [Header("HUD 참조")]
    [SerializeField] private HUD hud;

    [Header("보상 갯수")]
    [SerializeField] private int freeGoldAmount;
    [SerializeField] private int freeGemAmount;
    [SerializeField] private int freeStuffAmount;

    [Header("레벨업 재료 종류별로 ID")]
    [SerializeField] private List<string> levelUpStuffId = new List<string>(6);

    [Header("HUD 갱신 지연 시간")]
    [SerializeField] private float hudDelay = 1.0f;

    /// <summary>
    /// 젬 버튼 누를시, 보유젬개수 증가, HUD갱신
    /// </summary>
    public void OnClickGetGem()
    {
        SoundManager.Instance.PlaySound("GetCoin");
        PlayerInfo.gem += freeGemAmount;
        SaveManager.Instance.Save();
        StartCoroutine(RefreshHudDelayCo(hudDelay));
    }

    /// <summary>
    /// 골드 버튼 누를시, 보유골드개수 증가, HUD갱신
    /// </summary>
    public void OnClickGetGold()
    {
        SoundManager.Instance.PlaySound("GetCoin");
        PlayerInfo.gold += freeGoldAmount;
        SaveManager.Instance.Save();
        StartCoroutine(RefreshHudDelayCo(hudDelay));
    }

    /// <summary>
    /// 재료 버튼 누를시, 보유재료개수 증가
    /// </summary>
    public void OnClickGetStuff()
    {
        SoundManager.Instance.PlaySound("GetCoin");
        SaveManager.Instance.Save();
        GetLevelUpStuffAll(freeStuffAmount);
    }

    /// <summary>
    /// 재료 버튼 실제 로직
    /// 부위별(6종류) 재료 아이템 고정갯수로 지급
    /// </summary>
    /// <param name="amount"></param>
    private void GetLevelUpStuffAll(int amount)
    {
        if (PlayerInfo.stuffs == null)
        {
            PlayerInfo.stuffs = new List<StuffStack>();
        }
        
        //할당된 레벨업의 재료 ID 부분 순회
        for (int i = 0; i < levelUpStuffId.Count; i++)
        {
            string id = levelUpStuffId[i];

            //빈칸이면 스킵
            if (string.IsNullOrEmpty(id)) continue;

            //기존 StuffStack이랑 일치하는 Id 확인용 플래그
            bool found = false;

            //기존 stuffStack 이 있는경우
            //순회하면서 같은 Id 체크
            for (int j = 0; j < PlayerInfo.stuffs.Count; j++)
            {
                StuffStack stack = PlayerInfo.stuffs[j];

                //같은 id 있을시, 수량만 증가시키고 종료
                if (stack.stuff.id == id)
                {
                    stack.amount += amount;
                    found = true;
                    break;
                }
            }

            //기존 스택을 찾으면 해당 id 처리 끝
            if(found) continue;

            //stuffData 데이터 정의
            StuffData data = StuffDatabase.GetStuff(id);
            
            //원본 데이터 기반 런타임 데이터 생성
            StuffDataRuntime runtime = new StuffDataRuntime(data);

            //새 스택 생성해서 추가
            StuffStack newStack = new StuffStack();
            newStack.stuff = runtime;
            newStack.amount = amount;
            
            PlayerInfo.stuffs.Add(newStack);
        }
    }
    
    /// <summary>
    /// HUD 갱신할때 딜레이 걸어줄 코루틴
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator RefreshHudDelayCo(float time)
    {
        yield return new WaitForSeconds(time);
        hud.RefreshHUD(PlayerInfo);
    }
}
