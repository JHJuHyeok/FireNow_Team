using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CostDrain : MonoBehaviour
{
    [SerializeField] private GameObject costImage;
    [SerializeField] private RectTransform targetPosition;
    [SerializeField] private UnityEngine.UI.Button playButton;

    private Vector2[] _creationCostPosition;
    private Quaternion[] _creationCostRotation;

    private int _costAmount;

    private void Start()
    {

        _costAmount = costImage.transform.childCount;

        _creationCostPosition = new Vector2[_costAmount];
        _creationCostRotation = new Quaternion[_costAmount];

        for (int i = 0; i < _costAmount; i++)
        {
            GameObject temp = costImage.transform.GetChild(i).gameObject;
            _creationCostPosition[i] = temp.GetComponent<RectTransform>().anchoredPosition;
            _creationCostRotation[i] = temp.GetComponent<RectTransform>().rotation;
            temp.transform.localScale = Vector3.zero;
        }

    }

    public void CostParty()   //사용시 이 스크립트를 호출한다.
    {
        playButton.interactable = false;
        costImage.SetActive(true);
        var delay = 0f;

        float moveDruation = 0.8f;
        

        for (int i = 0; i < _costAmount; i++)
        {
            Transform cost = costImage.transform.GetChild(i);
            cost.gameObject.SetActive(true);
            //점점 커지며 등장.
            cost.DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);
            //지정된 좌표(해당하는 아이콘)으로 이동.
            cost.DOMove(targetPosition.position,moveDruation).SetDelay(delay + 0.5f).SetEase(Ease.InBack);
            //회전
            cost.DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f).SetEase(Ease.Flash);
            //퇴장
            cost.DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack);

            delay += 0.1f;

            targetPosition.transform.DOScale(1.1f, 0.1f).SetLoops(10, LoopType.Yoyo).SetEase(Ease.InOutSine).SetDelay(1.2f);
        }
        StartCoroutine(InitCost());
    }

    private void OnDisable()
    {
        playButton.interactable = true;
    }

    private IEnumerator InitCost()
    {
        //모든 애니메이션 끝날때까지 넉넉한 대기.
        yield return new WaitForSecondsRealtime(2f);

        for (int i = 0; i < _costAmount; i++)
        {
            Transform temp = costImage.transform.GetChild(i);

            //위치/회전 원상복구
            temp.GetComponent<RectTransform>().anchoredPosition = _creationCostPosition[i];
            temp.GetComponent<RectTransform>().rotation = _creationCostRotation[i];

            temp.gameObject.SetActive(false);
        }
        costImage.SetActive(false);
        playButton.interactable = true;
         
    }

   

}
