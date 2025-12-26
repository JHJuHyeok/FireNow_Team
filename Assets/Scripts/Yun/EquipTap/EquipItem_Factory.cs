using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//주혁님께서 EquipLoader 라고 만들어두신거, 테스트용 인듯? 그 구조에 맞게
//JSON읽는 로더 역할 따로, 그 정보로 아이템 객체를 만들어낼 팩토리가 필요
//JSON 직접 읽는게 아니라, JSON로더->데이터->팩토리 순서로
//지금은 안쓰고, 나중에 가챠 시스템 도입할때 추가로 작업해야 합니다. 데이터 연동 전의 상태에요
//public class EquipItem_Factory
//{
//    public static PlayerEquipItem Create(string jsonFileName, int gradeIndex = 0)
//    {
//        //1.데이터 JSON로더로 로드
//        EquipData data = TEST_EquipLoader.Load(jsonFileName);
//        if (data == null) return null;

//        //2.최초 등급(항상 0,normal) //create 할때 인덱스 부여 안하면 최초등급으로
//        EquipGrade gradeData = data.equipGrades[gradeIndex];

//        //3.아이콘 로드
//        Sprite icon = Resources.Load<Sprite>(data.iconPath);

//        //4.플레이어 장비 인스턴스 생성
//        return new PlayerEquipItem(data,gradeData,1,icon);
//    }
//}
