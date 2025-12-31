using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 진화 슬롯 클릭시 활성화될 패널 관리
/// 특정 슬롯의 evolveData 표시, 레벨테이블의 증가량과 해금비용 표시
/// 슬롯 클릭시 진화탭 컨트롤타워에 level 해금 요청
/// </summary>
public class EvolInfoPanel : MonoBehaviour
{
    [Header("진화 패널 텍스트 관련")]
    [SerializeField] private TextMeshProUGUI evolveName;
    [SerializeField] private TextMeshProUGUI evolveStatName;
    [SerializeField] private TextMeshProUGUI evolveValue;
    [SerializeField] private TextMeshProUGUI evolveDescript;

    [Header("해금 버튼 관련")]
    //해금 상태에 따라 활성화 상태 조절용
    [SerializeField] private GameObject unlockButtonRoot;
    [SerializeField] private Button unlockButton;
    [SerializeField] private TextMeshProUGUI unlockCost;

    //인포패널 닫기용 이벤트 핸들러 패널
    [Header("닫기 전용 이벤트 핸들러 패널")]
    [SerializeField] private GameObject closePanel;

    //현재 보고 있는 슬롯의 레벨
    private int _curLevel;

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
    public void Show(int level, EvolveData data, int value, int cost, bool isUnlocked, bool canUnlock)
    {
        //현재 슬롯의 레벨 저장
        _curLevel = level;
        
        //텍스트 및 버튼 세팅
        evolveName.text = data.evolveName;
        evolveStatName.text = ConvertGainStatToText(data.gainStat, data.nodeType);
        evolveValue.text = value.ToString();
        evolveDescript.text = data.descript;
        unlockCost.text = cost.ToString();
        unlockButtonRoot.SetActive((isUnlocked == false) && (canUnlock == true));
        
        gameObject.SetActive(true);
        closePanel.SetActive(true);
    }

    /// <summary>
    /// 진화 인포패널 숨김용
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
        closePanel.SetActive(false);
    }

    /// <summary>
    /// 해금 버튼 클릭시 호출
    /// </summary>
    private void OnClickUnlock()
    {
        //진화탭 컨트롤에 해금 요청
        _evolTabControl.TryUnlockSelectedSlot();
    }

    /// <summary>
    /// gainStat 키를 텍스트로 변환
    /// </summary>
    /// <param name="gainStat"></param>
    /// <param name="nodeType"></param>
    /// <returns></returns>
    private string ConvertGainStatToText(string gainStat, EvolveNodeType nodeType)
    {
        if (gainStat == "attack") return "공격력";
        if (gainStat == "maxHP") return "최대체력";
        if (gainStat == "defence") return "방어력";
        if (gainStat == "getHPWithMeat") return "고기회복";
        return gainStat;
    }
}
