using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ForceFieldSponer : MonoBehaviour
{
    [SerializeField] private GameObject spreadPrefab;

    private int count = 4; // 이거랑 아래것 손대지 마세요. 손대는 순간 애니메이션 망가집니다.
    private float interval = 2f;  // 총 프리펩 갯수와, 스폰 간격입니다.
    [SerializeField] private float maxScale = 0.7f; 
    [SerializeField] private bool _Eveloution = true; // 진화했다면 true 아니라면 False 를 하고 Restart() 호출

    private Coroutine _sequentialCoroutine;

    private void Start()
    {
        _sequentialCoroutine = StartCoroutine(Sequential());
    }

    public void Restart(float currentScale,bool Eveloution)
    {
        // 크기,진화여부에 변화를 주고싶다면 Restart(크기,진화여부)로 호출
        maxScale = currentScale;
        _Eveloution = Eveloution;
        if (_sequentialCoroutine != null)
        {
            StopCoroutine(_sequentialCoroutine);
            _sequentialCoroutine = null;
        }
        StartCoroutine(RestartRoutine());
        
    }

    private IEnumerator RestartRoutine()
    {
        DOTween.PauseAll();
        endSequential();
        yield return null;
        DOTween.PlayAll();
        _sequentialCoroutine = StartCoroutine(Sequential());
    }
    public void endSequential()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    private IEnumerator Sequential()
    {
        for (int i = 0; i<count; i++)
        {
            GameObject field = Instantiate(spreadPrefab, transform.position, Quaternion.identity, transform);
            SpriteRenderer spriteTenderer = field.GetComponent<SpriteRenderer>();
            
            if (_Eveloution == true)
            {
                spriteTenderer.color = new Color32(234, 116, 52, 255);
            }
            else
            {
                spriteTenderer.color = new Color32(77, 188, 79, 255);
            }

            if (i % 2 == 1)
            {
                field.transform.localRotation = Quaternion.Euler(0, 0, 22.5f);
            }

            Forcefield forcefield = field.GetComponent<Forcefield>();
            if (forcefield != null)
            {
                forcefield.spreadField(maxScale);
            }
            yield return new WaitForSeconds(interval);
        }
        
    }
}




