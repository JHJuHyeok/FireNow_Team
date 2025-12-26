
using UnityEngine;
using UnityEngine.UI;

public class BottomMenuBar : MonoBehaviour
{
    public GameObject[] menus;
    private LayoutElement[] _layouts;
    private Image[] _images;

    public Sprite[] normalSprites;
    public Sprite[] selectedSprites;

    public int currentTab = 0;
    public float defaultWidth = 1f;
    public float selectedWidth = 2f;
    public float lerpSpeed = 8f;

    private Image[] _iconImages;
    private GameObject[] _textObjects;
    public float iconNormalScale = 0.8f;
    public float iconSelectedScale = 3f;

    private float[] _targetWidths;

    private void Awake()
    {
        int count = menus.Length;

        _layouts = new LayoutElement[count];
        _images = new Image[count];
        _iconImages = new Image[count];
        _textObjects = new GameObject[count];
        _targetWidths = new float[count];
    }
    private void Start()
    {

        for (int i = 0; i < menus.Length; i++)
        {   //배경이미지
            _layouts[i] = menus[i].GetComponent<LayoutElement>();
            _images[i] = menus[i].GetComponent<Image>();
            // 아이콘은 메뉴의 첫번째 자손으로 설정.
            _iconImages[i] = menus[i].transform.GetChild(0).GetComponent<Image>();

            if (_iconImages[i].transform.childCount > 0)
            {
                _textObjects[i] = _iconImages[i].transform.GetChild (0).gameObject;
            }

            //아이콘 고정 가급적 위로 커지도록.
            _iconImages[i].rectTransform.pivot = new Vector2(0.5f, 0f);
            //스케일 초기화
            _iconImages[i].rectTransform.localScale = Vector3.one * iconNormalScale;

            _targetWidths[i] = defaultWidth;
        }


        _targetWidths[currentTab] = selectedWidth;
        UpdateTabImages();
    }

    public void Onclicktab(int index)
    {
        //같은 탭이면, 할것 없음.
        if (currentTab == index)
            return;
        //선택탭 번호 업데이트
        currentTab = index;

        //모든탭 너비 기본너비로 초기화
        for (int i = 0; i < _targetWidths.Length; i++)
        {
            _targetWidths[i] = defaultWidth;
        }

        //선택된 탭만 너비 증가.
        _targetWidths[index] = selectedWidth;
        UpdateTabImages();


    }

    private void Update()
    {
        // 현재 너비값을 목표 너비값으로 보간.
        for (int i = 0; i < _layouts.Length; i++)
        {
            _layouts[i].flexibleWidth = Mathf.Lerp(_layouts[i].flexibleWidth, _targetWidths[i], Time.deltaTime * lerpSpeed);

            float targetScale = (i == currentTab) ? iconSelectedScale : iconNormalScale;

            _iconImages[i].rectTransform.localScale = Vector3.Lerp(_iconImages[i].rectTransform.localScale,Vector3.one * targetScale,Time.deltaTime * lerpSpeed);
        }
    }
    private void UpdateTabImages()
    { //선택된 탭 이미지 노란색으로 변경.
        for (int i = 0; i < menus.Length; i++)
        {
            bool isSelected = (i == currentTab);
            //배경 스프라이트 변경.
            _images[i].sprite = isSelected ? selectedSprites[i] : normalSprites[i];

            //텍스트 활설화 /비활성화
            if (_textObjects[i] != null)
            {
                _textObjects[i].SetActive(isSelected);
            }
        }
    }
}
