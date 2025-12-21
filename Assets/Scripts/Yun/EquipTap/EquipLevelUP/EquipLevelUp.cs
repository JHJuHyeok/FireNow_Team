using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// 공용 강화재료 ID 기반 레벨업 시스템
/// 비용조회, 재화 체크, 차감, 레벨증가, 실패알림 반환용
/// (인포패널이 괴물스크립트가 되어가고 있어서, 따로 레벨업 로직분리)
/// -EquipData에 레벨업에 필요한 재료ID 정보가 없어서
/// 모든 장비가 동일한 재료 ID를 사용한다고 일단 가정.
/// 나중에 필드 추가시 재료ID만 교체하면 되는식으로
/// </summary>
public class EquipLevelUp
{
    //임시 강화재료 ID(stuffData.id와 동일해야함)
    public const string tempStuffId = "Scroll_Weapon";

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

    //레벨업 가능여부 검사
    public static CheckLevelUpResult Check(Equip_ItemBase item, PlayerInfoSO playerInfo)
    {
        CheckLevelUpResult result = new CheckLevelUpResult();
        //기본틀
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

        int haveGold = playerInfo.gold;
        int haveStuff = GetStuffAmount(playerInfo, tempStuffId);

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
        //레벨업 불가시
        if (check.canLevelUp == false)
        {
            text = check.alertText;
            return false;
        }

        EquipInfoBridge bridge = item as EquipInfoBridge;
        EquipLevelUpCost cost = CostTable.GetCost(check.nextLevel);

        //보유 골드 차감
        playerInfo.gold = playerInfo.gold - cost.requiredGold;
        //보유 재료 차감시도 해보고, 안되면 골드 돌려주기
        bool spend = TrySpendStuff(playerInfo, tempStuffId, cost.stuffCount);
        if (spend == false)
        {
            playerInfo.gold = playerInfo.gold + cost.requiredGold;
            text = "재료부족알림";
            return false;
        }
        //레벨증가(여기서 진짜 데이터는 EquipInfo.Level)
        bridge.ItemBaseSourceInfo.level = bridge.ItemBaseSourceInfo.level + 1;

        text = "레벨업성공알림";
        return true;
    }

    //특정 재료 ID의 보유 수량 반환 함수
    private static int GetStuffAmount(PlayerInfoSO playerInfo, string stuffId)
    {
        if (playerInfo == null) return 0;
        if (playerInfo.stuffs == null) return 0;

        for (int i = 0; i < playerInfo.stuffs.Count; i++)
        {
            StuffStack stack = playerInfo.stuffs[i];
            if (stack == null || stack.stuff == null) //자꾸 터져서 방어코드 
            {
                continue;
            }
            
            if (stack.stuff.id == stuffId)
            {
                return stack.amount;
            }
        }
        return 0;
    }

    //특정 재료 사용, amount만큼 차감하는 함수
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
}
