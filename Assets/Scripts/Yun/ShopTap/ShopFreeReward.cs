using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//1.재화,재료 지급은 버튼 클릭시 바로 지급하게
//2.애니메이션이 끝나는 타이밍을 직접 딜레이로 잡아서 HUD갱신 함수 호출
//3.버튼 무한클릭 방지는 이미 애니메이션 코드쪽에서 제어중이니까 중복처리 하지 말것.
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

    //레벨업 재료 종류 6개
    [Header("레벨업 재료 종류별로 ID")]
    [SerializeField] private List<string> levelUpStuffId = new List<string>(6);

    //HUD갱신 타이밍 잡을 시간 딜레이
    [Header("HUD 갱신 지연 시간")]
    [SerializeField] private float hudDelay = 1.0f;

    /// <summary>
    /// 젬 버튼 눌렀을때 온클릭
    /// </summary>
    public void OnClickGetGem()
    {
        //플레이어 인포쪽에 바로 젬갯수만큼 누적 시켜주고,
        PlayerInfo.gem += freeGemAmount;
        //딜레이 걸고,HUD 갱신 코루틴
        StartCoroutine(RefreshHudDelayCo(hudDelay));
    }

    /// <summary>
    /// 골드 버튼 눌렀을때 온클릭
    /// </summary>
    public void OnClickGetGold()
    {
        PlayerInfo.gold += freeGoldAmount;
        StartCoroutine(RefreshHudDelayCo(hudDelay));
    }
    
    //재료 버튼 눌렀을때 온클릭
    public void OnClickGetStuff()
    {
        GetLevelUpStuffAll(freeStuffAmount);
    }

    //이제 재료버튼 눌렀을때 기능인데,
    //재료 아이템 종류전부를 지급하는 구조로->
    //갯수만 변경할 수 있게
    private void GetLevelUpStuffAll(int amount)
    {
        //플레이어 인포 쪽에 소지품리스트 없을 경우
        if (PlayerInfo.stuffs == null)
        {
            //->새로 생성해주고
            PlayerInfo.stuffs = new List<StuffStack>();
        }
        //여기서 할당된 레벨업의 재료 ID 알아야 되니까 그부분 먼저 순회하고,
        for (int i = 0; i < levelUpStuffId.Count; i++)
        {
            string id = levelUpStuffId[i];

            //빈칸이면 스킵
            if (string.IsNullOrEmpty(id)) continue;

            //기존 StuffStack이랑 일치하는 Id 찾았는지 플래그 세워두기
            bool found = false;

            //기존 stuffStack 이 있는경우
            //순회하면서 같은 Id찾기,
            for (int j = 0; j < PlayerInfo.stuffs.Count; j++)
            {
                StuffStack stack = PlayerInfo.stuffs[j];

                //기존 리스트에 비정상 데이터 있으면 스킵(스터프제이슨 의심됨)-임시
                //if (stack == null || stack.stuff == null) continue;

                //같은 id 찾으면, 수량 증가시키고 종료
                if (stack.stuff.id == id)
                {
                    stack.amount += amount;
                    found = true;
                    break;
                }
            }

            //기존 스택을 찾으면 해당 id 처리 끝
            if(found) continue;

            //DB에서 원본 stuffData를 가져오고
            StuffData data = StuffDatabase.GetStuff(id);
            
            //원본 데이터를 기반으로 런타임 데이터 생성
            StuffDataRuntime runtime = new StuffDataRuntime(data);

            //새 스택 생성해서 추가
            StuffStack newStack = new StuffStack();
            newStack.stuff = runtime;
            newStack.amount = amount;
            
            PlayerInfo.stuffs.Add(newStack);
        }
    }
    
    //HUD 갱신할때 딜레이 걸어줄 코루틴 필요
    private IEnumerator RefreshHudDelayCo(float time)
    {
        //딜레이 걸고
        yield return new WaitForSeconds(time);
        //HUD 갱신
        hud.RefreshHUD(PlayerInfo);
    }
}
