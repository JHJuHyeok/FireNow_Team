using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 테스트용 장비재료 지급기
/// PlayerInfoSo에 특정 재료를 ID로 지급
/// 런타임기반이라 인스펙터에서 채우기 불가
/// 데이터베이스에서 stuffdata 찾아서 런타임 객체로 만들어서 지급해버릴거야
/// </summary>

public class Test_AddStuff : MonoBehaviour
{
    [Header("런타임 단일 소스")]
    [SerializeField] private PlayerInfoSO playerInfo;

    [Header("지급할 재료ID (StuffData.id)")]
    [SerializeField] private string stuffId = "Scroll_Weapon";

    [Header("지급 개수")]
    [SerializeField] private int amount = 10;

    private void Start()
    {
        if (playerInfo == null)
        {
            Debug.LogError("playerInfo가 null 상태.");
            return;
        }

        if (playerInfo.stuffs == null)
        {
            playerInfo.stuffs = new List<StuffStack>();
        }

        //DB에서 StuffData 조회 (Initializer로 StuffDatabase.Initialize가 되어 있어야 함)
        StuffData stuffData = StuffDatabase.GetStuff(stuffId);
        if (stuffData == null)
        {
            Debug.LogError("StuffDatabase에서 재료id를 찾지 못함!: " + stuffId);
            return;
        }

        //런타임 데이터 생성
        StuffDataRuntime runtime = new StuffDataRuntime(stuffData);

        //이미 있으면 누적
        for (int i = 0; i < playerInfo.stuffs.Count; i++)
        {
            StuffStack stack = playerInfo.stuffs[i];
            if (stack == null || stack.stuff == null) continue;

            if (stack.stuff.id == stuffId)
            {
                stack.amount = stack.amount + amount;
                Debug.Log("재료 지급 완료(누적++): " + stuffId + " +" + amount + " = " + stack.amount);
                return;
            }
        }

        //없으면 신규 추가
        StuffStack newStack = new StuffStack();
        newStack.stuff = runtime;
        newStack.amount = amount;

        playerInfo.stuffs.Add(newStack);
        Debug.Log("재료 지급 완료(신규+): " + stuffId + " +" + amount);
    }
}
