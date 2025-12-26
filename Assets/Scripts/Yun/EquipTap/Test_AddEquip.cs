using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 장비 인벤토리 시스템 데이터 연동 테스트 위한 스크립트
/// 데이터 관련 변동 없으면 디폴트로 쓰다가 최종빌드 전에 삭제할것
///
/// 해당 스크립트 주 목적>
/// -장비 인벤토리 시스템이 "데이터->브릿지->UI->장착/해제" 까지
/// 정상적으로 연결되어 있는지 확인용 스크립트입니다.
/// 
/// -SaveManager에 저장되어있는 상태가 아닐때만 사용가능
/// -가챠 상점 합성 구현할때는 꺼줘야 됩니다!
/// </summary>
public class Test_AddEquip : MonoBehaviour
{
    //진짜 보유 장비 리스트를 들고있는 인벤토리 데이터
    [Header("런타임 단일 소스")]
    [SerializeField] private PlayerInfoSO playerInfoSO;

    //장비 장착/해제 처리하는 인벤토리 UI갱신 담당의 EquipControl
    [Header("장착/해제 UI 갱신용")]
    [SerializeField] private EquipControl equipControl;

    //인스펙터에서 ID값을 넣을때, 해당경로에 있는 JSON의 정확한 이름을 써줘야됩니다.
    [Header("테스트로 넣을 장비 id 목록 (EquipData.id 값)")]
    [SerializeField] private List<string> testEquipIDs = new List<string>();

    //테스트로 추가할 장비의 등급설정
    [Header("테스트 등급")]
    [SerializeField] private Grade testGrade = Grade.normal;

    //테스트로 추가할 장비의 시작 레벨 설정
    [Header("테스트 레벨 지정")]
    [SerializeField] private int testLevel = 1;

    //true -> 기존인벤토리를 모두 비우고 테스트 장비만 추가할때 사용
    [Header("기존 인벤을 비우고 시작할지?")]
    [SerializeField] private bool clearInventoryFirst = true;

    private void Start()
    {
        //장비 데이터 베이스 초기화
        //EquipDatabase는 내부에 딕셔너리를 사용중이셔서,
        //반드시 초기화 함수가 먼저 호출되어야 해요
        EquipDatabase.Initialize();

        //SaveManager.Load() 결과에 따라
        //equips 리스트가 null일 수 있는거 방어코드
        if (playerInfoSO.equips == null)
        {
            playerInfoSO.equips = new List<EquipInfo>();
        }

        //테스트 시작 전에 인벤토리 초기화 여부에 따라 초기화
        if (clearInventoryFirst)
        {
            playerInfoSO.equips.Clear();
        }

        //장비 추가
        for (int i = 0; i < testEquipIDs.Count; i++)
        {
            string equipID = testEquipIDs[i];

            //장비정의 데이터 들고오기
            EquipData equipData = EquipDatabase.GetEquip(equipID);

            //런타임용 장비정의 데이터 생성
            EquipDataRuntime runtime = new EquipDataRuntime(equipData);

            //플레이어 보유 장비 데이터 생성
            EquipInfo info = new EquipInfo();
            info.equip = runtime;
            info.grade = testGrade;
            info.level = testLevel;

            //진짜 인벤 데이터에 추가
            playerInfoSO.equips.Add(info);
        }

        //UI갱신
        equipControl.RefreshInventoryUI();
    }
}
