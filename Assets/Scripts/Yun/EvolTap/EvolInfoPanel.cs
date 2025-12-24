using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//슬롯 프리팹의 버튼 클릭시 등장 패널
//패널에서 띄울거
//-노드이름(EvolveData.EvolveName),
//-스탯이름(BasicEvolRule.GetStatNameToText),
//-능력치 수치(BasicEvolRule.GetIncreaseAmountByStep),
//-노드 설명 텍스트(EvolveData.descript),
//-진화버튼(해금 가능시에만 표시),
//-버튼 비용텍스트(BasicEvolRule.unlockEvolveCost) 
public class EvolInfoPanel : MonoBehaviour
{
    [Header("진화 패널 텍스트 관련")]
    //슬롯이름
    [SerializeField] private TextMeshProUGUI evolveName;
    //스탯이름
    [SerializeField] private TextMeshProUGUI evolveStatName;
    //능력치 수치
    [SerializeField] private TextMeshProUGUI evolveValue;
    //슬롯설명
    [SerializeField] private TextMeshProUGUI evolveDescript;

    [Header("해금 버튼 관련")]
    //해금 상태에 따라 활성화 상태 조절용
    [SerializeField] private GameObject unlockButtonRoot;
    [SerializeField] private Button unlockButton;
    [SerializeField] private TextMeshProUGUI unlockCost;

    //현재 보고 있는 슬롯의 인덱스 파악용
    private int _curSlotIndex;

    //버튼 클릭시 컨트롤에 해금요청 보낼용도의 참조
    private EvolTabControl _evolTabControl;

    /// <summary>
    /// 패널이 컨트롤로 해금요청 보낼수 있게 바인딩
    /// </summary>
    /// <param name="control"></param>
    public void BindControl(EvolTabControl control)
    {
        //참조 저장
        _evolTabControl = control;
        //버튼 이벤트 해제 및 등록
        unlockButton.onClick.RemoveAllListeners();
        unlockButton.onClick.AddListener(OnClickUnlock);
    }

    /// <summary>
    /// 패널 표시, 내용 갱신용
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="data"></param>
    /// <param name="statType"></param>
    /// <param name="amount"></param>
    /// <param name="isUnlocked"></param>
    /// <param name="canUnlock"></param>
    public void Show(int slotIndex, EvolveData data, BasicEvolveStatType statType, int amount, bool isUnlocked, bool canUnlock)
    {
        //현재 슬롯의 인덱스 저장
        _curSlotIndex = slotIndex;
        //슬롯 이름 세팅
        evolveName.text = data.evolveName;
        //스탯 이름 세팅
        evolveStatName.text = BasicEvolRule.GetStatNameToText(statType);
        //스탯 수치 세팅
        evolveValue.text = amount.ToString();
        //슬롯 설명 세팅
        evolveDescript.text = data.descript;
        //비용 세팅
        unlockCost.text = BasicEvolRule.unlockEvolveCost.ToString();
        //버튼 표시 세팅(해금상태에만 활성화)
        unlockButtonRoot.SetActive((isUnlocked == false) && (canUnlock == true));
        //해당패널 활성화
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 단순 패널 숨김용
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 해금 버튼 클릭시 호출
    /// </summary>
    private void OnClickUnlock()
    {
        //컨트롤에 해금 요청
        _evolTabControl.TryUnlock(_curSlotIndex);
    }
}
