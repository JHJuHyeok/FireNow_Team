using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//테스트용 아이템들을 만들어주는 클래스
//임시용이므로, 추후 JSON 또는 SO에서 데이터를 로드할 예정-지운다는 소리
public class Test_ItemGenerator : MonoBehaviour
{
    public Sprite[] itemIcons;
    public Sprite[] partIcons;

    public List<Equip_ItemBase> items = new List<Equip_ItemBase>();

    private void Awake()
    {
        //임시로 무기 아이템을 만들어보자
        ItemData_Test Weapon = new ItemData_Test();
        Weapon.itemID = 101;
        Weapon.itemName = "임시 무기";
        Weapon.grade = ItemGrade.Normal;
        Weapon.description = "테스트용 임시무기";
        Weapon.attackPower = 1;
        Weapon.itemLevel = 1;
        Weapon.itemIcon = itemIcons[0];
        Weapon.needPartIcon = partIcons[0];
        Weapon.needPartCount = 1;
        Weapon.havePartCount = 1;
        Weapon.needCoin = 1;
        Weapon.haveCoin = 1;
        Weapon.canEquip = true;
        Weapon.equipSlotType = EquipSlotType.Weapon;

        items.Add(Weapon);

        //글로브 체크용
        ItemData_Test glove = new ItemData_Test();
        glove.itemID = 102;
        glove.itemName = "임시 무기2";
        glove.grade = ItemGrade.Rare;
        glove.description = "테스트용 임시무기2";
        glove.attackPower = 2;
        glove.itemLevel = 3;
        glove.itemIcon = itemIcons[2];
        glove.needPartIcon = partIcons[2];
        glove.needPartCount = 4;
        glove.havePartCount = 5;
        glove.needCoin = 6;
        glove.haveCoin = 7;
        glove.canEquip = true;
        glove.equipSlotType = EquipSlotType.Glove;

        items.Add(glove);
        //타입별 슬롯 정상작동

        //이후 수정할것
        //1.아이템 등급 별 이미지 변경 추가
        //-인포패널에서 상단 이미지 1개
        //-인포패널에서 아이콘 이미지 1개+(테두리용) ->슬롯 자체를 조금 변경할 필요 있음
        //현재 인포패널의 아이콘에는 이미지 칸이 1개만 들어가있음 테두리용+ 분리할것
        //등급별 이미지 매핑 필요 - > SO로 일단 해보자 
        //등급별 테두리, 상단이미지 ItemGrade 별 이미지 매핑해서 SO로 만들어두고
        //해당 SO를 장착슬롯, 인벤토리 슬롯에서 가져오는식으로 하면 되지않을까
        //그러면 ItemBase 에서 가져오는게 아니니까 지워줘야되고,(등급이름만 가져오는거니까)
        //등급 이미지 맵핑은 단독으로 움직이는 형태가 되어야해

        //2.장비 슬롯 장착해제시 이미지 복구 추가 - 테두리, 원본아이콘 2개

        //3.장비 타입별 공격력/체력 아이콘 변경 - 

        //4.장비 등급별 상세 설명 맵핑 추가
        //- 등급 3개 할거니까, 필요 등급스킬 설명 텍스트도 3개 

        //이후 테스트 할것-
        //1.SO데이터 임시로 만들어서 확인,
        //2.JSON 데이터 임시로 만들어서 확인

        //여기까지 진행후에, 데이터 형식 보고 수정해야함+ 플레이어 능력치 변경 관련해서도
    }
}
