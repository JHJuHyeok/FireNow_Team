using DG.Tweening;
using UnityEngine;
using System.Collections;

public class BagAppearance : MonoBehaviour
{

    [SerializeField] private GameObject Bag;
    [SerializeField] private RectTransform bagEndPosition;
    [SerializeField] private RectTransform bagStartPosition;

    [SerializeField] private GameObject costImage;
    [SerializeField] private RectTransform targetPosition;

    private Transform _bagPosition;
    private Vector2[] _creationCostPosition;
    private Quaternion[] _creationCostRotation;

    private int _costAmount;

    private void Awake()
    {
        _bagPosition = Bag.transform;
    }
    void Start()
    {
        Bag.SetActive(false);
        
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


    public void BagAppearsStart()
    {
        // 가방 등장.
        Bag.SetActive(true);
        // 가방 미끄려져 나오기
        _bagPosition.DOMove(bagStartPosition.position, 0.5f).SetEase(Ease.InBack);
        // 자원 생성,빨려들어가기
        CostParty();
    }
    public void BagAppearsEnd()
    {
        //미끄러져 들어가기
        _bagPosition.DOMove(bagEndPosition.position, 0.5f).SetEase(Ease.InBack)
            //애니메이션 종료 후 실행
            .OnComplete(() =>
            {   //가방 비활성화. 
                Bag.SetActive(false);
            });
        
    }

    public void CostParty()
    {
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
            cost.DOMove(targetPosition.position, moveDruation).SetDelay(delay + 0.5f).SetEase(Ease.InBack);
            //회전
            cost.DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f).SetEase(Ease.Flash);
            //퇴장
            cost.DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack);

            delay += 0.1f;

            targetPosition.transform.DOScale(1.1f, 0.1f).SetLoops(10, LoopType.Yoyo).SetEase(Ease.InOutSine).SetDelay(1.2f);
        }
        StartCoroutine(InitCost());
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
        yield return new WaitForSecondsRealtime(0.2f);
        //0.2초 대기후, 가방 퇴장 실행
        BagAppearsEnd();
    }
}
