using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// 각 부위별 강화재료 ID 기반 레벨업 시스템
/// 비용조회, 재화 체크, 차감, 레벨증가, 실패알림 반환용
/// </summary>
public class EquipLevelUp
{
    //레벨업 체크 결과 인포패널UI에 전달해줄 구조체
    public class CheckLevelUpResult
    {
        //레벨업 가능여부 플래그
        public bool canLevelUp;
        
        //레벨업 불가시 띄울 알림텍스트
        public string alertText;
        
        //다음 레벨
        public int nextLevel;
        
        //필요재화
        public int needGold;
        public int needStuff;
        public int haveGold;
        public int haveStuff;
        
        //재화계산후 남은것
        public int lackGold;
        public int lackStuff;
    }

    /// <summary>
    /// 레벨업 가능여부 검사
    /// </summary>
    /// <param name="item"></param>
    /// <param name="playerInfo"></param>
    /// <returns></returns>
    public static CheckLevelUpResult Check(Equip_ItemBase item, PlayerInfoSO playerInfo)
    {
        CheckLevelUpResult result = new CheckLevelUpResult();
        result.canLevelUp = false;
        result.alertText = "";
        result.nextLevel = 0;
        result.needGold = 0;
        result.needStuff = 0;
        result.haveGold = 0;
        result.haveStuff = 0;
        result.lackGold = 0;
        result.lackStuff = 0;

        //만렙 제한
        if (item.Level >= item.MaxLevel)
        {
            result.alertText = "최대레벨알림";
            return result;
        }

        int nextlevel = item.Level + 1;
        EquipLevelUpCost cost = CostTable.GetCost(nextlevel);

        //보유 골드, 보유 재료
        int haveGold = playerInfo.gold;

        //각 부위별 필요 재료 필드 추가된걸로 변경
        string requierdStuffId = GetRequiredStuffID(item);
        int haveStuff = GetStuffAmount(playerInfo, requierdStuffId);

        result.nextLevel = nextlevel;
        result.needGold = cost.requiredGold;
        result.needStuff = cost.stuffCount;
        result.haveGold = haveGold;
        result.haveStuff = haveStuff;

        int lackGold = cost.requiredGold - haveGold;
        int lackstuff = cost.stuffCount - haveStuff;

        if (lackGold < 0) lackGold = 0;
        if (lackstuff < 0) lackstuff = 0;

        result.lackGold = lackGold;
        result.lackStuff = lackstuff;

        if (lackGold == 0 && lackstuff == 0)
        {
            result.canLevelUp = true;
            result.alertText = "";
        }
        else
        {
            result.canLevelUp = false;
            result.alertText = "재화부족알림";
        }
        return result;
    }

    /// <summary>
    /// 레벨업 기능 함수
    /// 성공시 재화 차감하고, 레벨++
    /// 실패시 false 반환하고, 실패이유 텍스트로 표시
    /// </summary>
    /// <param name="item"></param>
    /// <param name="playerInfo"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool TryLevelUp(Equip_ItemBase item, PlayerInfoSO playerInfo, out string text)
    {
        text = "";

        CheckLevelUpResult check = Check(item, playerInfo);
        
        //레벨업 불가 시 상황에 맞는 텍스트 표시
        if (check.canLevelUp == false)
        {
            text = check.alertText;
            return false;
        }

        EquipInfoBridge bridge = item as EquipInfoBridge;
        EquipLevelUpCost cost = CostTable.GetCost(check.nextLevel);

        //보유 골드 차감
        playerInfo.gold = playerInfo.gold - cost.requiredGold;
        
        //보유 재료 차감시도 이후, 실패시 골드 반환
        string requiredStuffId = GetRequiredStuffID(item);
        bool spend = TrySpendStuff(playerInfo, requiredStuffId, cost.stuffCount);
        if (spend == false)
        {
            playerInfo.gold = playerInfo.gold + cost.requiredGold;
            text = "재료부족알림";
            return false;
        }

        //레벨증가
        bridge.ItemBaseSourceInfo.level = bridge.ItemBaseSourceInfo.level + 1;

        text = "레벨업성공알림";
        return true;
    }

    /// <summary>
    /// 특정 재료 ID의 보유 수량 반환
    /// </summary>
    /// <param name="playerInfo"></param>
    /// <param name="stuffId"></param>
    /// <returns></returns>
    private static int GetStuffAmount(PlayerInfoSO playerInfo, string stuffId)
    {
        if (playerInfo == null) return 0;
        if (playerInfo.stuffs == null) return 0;

        for (int i = 0; i < playerInfo.stuffs.Count; i++)
        {
            StuffStack stack = playerInfo.stuffs[i];
            
            if (stack == null || stack.stuff == null) continue;
            if (stack.stuff.id == stuffId) return stack.amount;
        }
        return 0;
    }

    /// <summary>
    /// 레벨업 시 특정 재료 사용 시도, 성공시 amount만큼 차감
    /// </summary>
    /// <param name="playerInfo"></param>
    /// <param name="stuffId"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    private static bool TrySpendStuff(PlayerInfoSO playerInfo, string stuffId, int amount)
    {
        if (amount <= 0) return true;
        if (playerInfo == null) return false;

        if (playerInfo.stuffs == null)
        {
            //리스트없으면 차감불가
            return false;
        }

        for (int i = 0; i < playerInfo.stuffs.Count; i++)
        {
            StuffStack stack = playerInfo.stuffs[i];
            if (stack == null) continue;
            if (stack.stuff == null) continue;

            if (stack.stuff.id == stuffId)
            {
                if (stack.amount < amount) return false;
                stack.amount = stack.amount - amount;

                if (stack.amount <= 0)
                {
                    playerInfo.stuffs.RemoveAt(i);
                }
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 재료 사용부분-부위별 필요재료Id 가져올 함수
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private static string GetRequiredStuffID(Equip_ItemBase item)
    {
        EquipInfoBridge bridge = item as EquipInfoBridge;
        if (bridge == null) return null;
        if (bridge.SourceEquipData == null) return null;

        return bridge.SourceEquipData.requiredStuffId;
    }
}
