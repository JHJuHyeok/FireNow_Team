using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 진화탭 진화 슬롯 전용 버튼 스크립트
/// 해금상태에 따라 이미지(테두리 포함)변경,
/// 버튼 클릭시 컨트롤에 클릭이벤트 전달
/// </summary>
public class EvolSlotButton : MonoBehaviour
{
    [Header("슬롯 프리팹 버튼")]
    [SerializeField] private Button evolveSlotButton;

    [Header("테두리,아이콘 이미지")]
    [SerializeField] private Image borderImage;
    [SerializeField] private Image iconImage;
    
    [Header("해금상태별 테두리 스프라이트")]  
    [SerializeField] private Sprite unlockedBorderSprite;
    [SerializeField] private Sprite lockedBorderSprite;

    //선택된 슬롯 인덱스 저장용
    private int _slotIndex;
    
    //클릭 이벤트 전달할 컨트롤 참조
    private EvolTabControl _evolTabControl;

    //슬롯 생성 이후 진화탭 컨틀롤에서 호출해서 초기화
    public void Initialize(int slotIndex, EvolTabControl evolTabControl)
    {
        //인덱스 저장
        this._slotIndex = slotIndex;
        //컨트롤 참조 저장
        this._evolTabControl = evolTabControl;

        evolveSlotButton.onClick.RemoveAllListeners();
        evolveSlotButton.onClick.AddListener(OnclickSlot);
    }
    /// <summary>
    /// 슬롯 UI갱신
    /// 테두리 부분은 공용2개중 선택,
    /// 아이콘 부분은 컨트롤이 넘겨준 스프라이트로 변경
    /// </summary>
    /// <param name="isUnlocked"></param>
    /// <param name="unlockedIcon"></param>
    /// <param name="lockedIcon"></param>
    public void RefreshSlotUI(bool isUnlocked, Sprite unlockedIcon, Sprite lockedIcon)
    {
        //테두리 스프라이트 처리
        borderImage.sprite = isUnlocked ? unlockedBorderSprite : lockedBorderSprite;

        //아이콘 스프라이트 처리부분
        if (isUnlocked)
        {
            iconImage.sprite = unlockedIcon;
        }
        else
        {
            iconImage.sprite = lockedIcon;
        }
    }
    /// <summary>
    /// 슬롯 버튼 클릭시 호출
    /// 해당 슬롯의 RectTransform도 컨트롤로 같이 전달
    /// </summary>
    private void OnclickSlot()
    {
        RectTransform rectTransform;
        //RectTransform 으로 캐스팅
        rectTransform = transform as RectTransform;
        //컨트롤로 클릭 전달
        _evolTabControl.OnClickSlot(_slotIndex, rectTransform);
    }
}
